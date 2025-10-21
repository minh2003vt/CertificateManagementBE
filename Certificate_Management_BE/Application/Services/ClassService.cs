using Application.Dto.ClassDto;
using Application.Dto.GradeImportDto;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICertificateEligibilityService _certificateEligibilityService;

        public ClassService(IUnitOfWork unitOfWork, ICertificateEligibilityService certificateEligibilityService)
        {
            _unitOfWork = unitOfWork;
            _certificateEligibilityService = certificateEligibilityService;
        }

        public async Task<ServiceResponse<List<ClassListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<ClassListDto>>();
            try
            {
                var classes = await _unitOfWork.ClassRepository.GetAll();
                var users = await _unitOfWork.UserRepository.GetAll();
                var instructorMap = users.ToDictionary(u => u.UserId, u => u.FullName);
                response.Data = classes
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new ClassListDto
                    {
                        ClassId = c.ClassId,
                        InstructorId = c.InstructorId,
                        SubjectId = c.SubjectId,
                        ClassGroupId = c.ClassGroupId,
                        InstructorName = instructorMap.ContainsKey(c.InstructorId) ? instructorMap[c.InstructorId] : string.Empty,
                        CreatedAt = c.CreatedAt
                    }).ToList();
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<ClassDetailDto>> GetByIdAsync(int classId)
        {
            var response = new ServiceResponse<ClassDetailDto>();
            try
            {
                var c = await _unitOfWork.ClassRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassId == classId);
                if (c == null)
                {
                    response.Success = false;
                    response.Message = "Class not found";
                    return response;
                }
                var instructor = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == c.InstructorId);
                response.Data = new ClassDetailDto
                {
                    ClassId = c.ClassId,
                    InstructorId = c.InstructorId,
                    SubjectId = c.SubjectId,
                    InstructorName = instructor?.FullName ?? string.Empty,
                    ClassGroupId = c.ClassGroupId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                };
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<ClassDetailDto>> CreateAsync(CreateClassDto dto, string createdByUserId)
        {
            var response = new ServiceResponse<ClassDetailDto>();
            try
            {
                // Validate Instructor exists and has RoleId = 2
                var instructor = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == dto.InstructorId);
                if (instructor == null)
                {
                    response.Success = false;
                    response.Message = "Instructor not found";
                    return response;
                }
                if (instructor.RoleId != 2)
                {
                    response.Success = false;
                    response.Message = "InstructorId must belong to a user with role Instructor (RoleId=2)";
                    return response;
                }

                // Validate Subject exists
                var subject = await _unitOfWork.SubjectRepository.GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == dto.SubjectId);
                if (subject == null)
                {
                    response.Success = false;
                    response.Message = "Subject not found";
                    return response;
                }

                // Validate ClassGroup exists
                var classGroup = await _unitOfWork.ClassGroupRepository.GetSingleOrDefaultByNullableExpressionAsync(g => g.ClassGroupId == dto.ClassGroupId);
                if (classGroup == null)
                {
                    response.Success = false;
                    response.Message = "ClassGroup not found";
                    return response;
                }

                // Validate instructor is assigned to teach this subject
                var canTeach = await _unitOfWork.InstructorAssignationRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(ia => ia.InstructorId == dto.InstructorId && ia.SubjectId == dto.SubjectId);
                if (canTeach == null)
                {
                    response.Success = false;
                    response.Message = "Instructor is not assigned to teach this subject";
                    return response;
                }

                // Ensure no other class in the same group teaches the same subject
                var duplicate = await _unitOfWork.ClassRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.ClassGroupId == dto.ClassGroupId && c.SubjectId == dto.SubjectId);
                if (duplicate != null)
                {
                    response.Success = false;
                    response.Message = "A class for this subject already exists in the selected class group";
                    return response;
                }

                var entity = new Class
                {
                    InstructorId = dto.InstructorId,
                    SubjectId = dto.SubjectId,
                    ClassGroupId = dto.ClassGroupId,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.ClassRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = (await GetByIdAsync(entity.ClassId)).Data;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<ClassDetailDto>> UpdateAsync(int classId, UpdateClassDto dto, string updatedByUserId)
        {
            var response = new ServiceResponse<ClassDetailDto>();
            try
            {
                var entity = await _unitOfWork.ClassRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassId == classId);
                if (entity == null)
                {
                    response.Success = false;
                    response.Message = "Class not found";
                    return response;
                }
                // Validate inputs
                var subject = await _unitOfWork.SubjectRepository.GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == dto.SubjectId);
                if (subject == null)
                {
                    response.Success = false;
                    response.Message = "Subject not found";
                    return response;
                }
                var classGroup = await _unitOfWork.ClassGroupRepository.GetSingleOrDefaultByNullableExpressionAsync(g => g.ClassGroupId == dto.ClassGroupId);
                if (classGroup == null)
                {
                    response.Success = false;
                    response.Message = "ClassGroup not found";
                    return response;
                }

                // Ensure current instructor can teach the (potentially new) subject
                var canTeach = await _unitOfWork.InstructorAssignationRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(ia => ia.InstructorId == entity.InstructorId && ia.SubjectId == dto.SubjectId);
                if (canTeach == null)
                {
                    response.Success = false;
                    response.Message = "Instructor is not assigned to teach this subject";
                    return response;
                }

                // Prevent duplicate class with same ClassGroup and Subject
                var duplicate = await _unitOfWork.ClassRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.ClassGroupId == dto.ClassGroupId && c.SubjectId == dto.SubjectId && c.ClassId != classId);
                if (duplicate != null)
                {
                    response.Success = false;
                    response.Message = "A class for this subject already exists in the selected class group";
                    return response;
                }

                entity.SubjectId = dto.SubjectId;
                entity.ClassGroupId = dto.ClassGroupId;
                entity.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.ClassRepository.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = (await GetByIdAsync(entity.ClassId)).Data;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int classId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var entity = await _unitOfWork.ClassRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassId == classId);
                if (entity == null)
                {
                    response.Success = false;
                    response.Message = "Class not found";
                    return response;
                }

                await _unitOfWork.ClassRepository.DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                response.Success = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        #region ImportGrades
        public async Task<ServiceResponse<ImportGradeResultDto>> ImportGradesAsync(int classId, IFormFile file)
        {
            var response = new ServiceResponse<ImportGradeResultDto>();
            var result = new ImportGradeResultDto();

            try
            {
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Message = "File is empty or not provided";
                    return response;
                }

                // Check if class exists and get subject requirements
                var classEntity = await _unitOfWork.ClassRepository.GetSingleOrDefaultByNullableExpressionAsync(c => c.ClassId == classId);
                if (classEntity == null)
                {
                    response.Success = false;
                    response.Message = "Class not found.";
                    return response;
                }

                var subject = await _unitOfWork.SubjectRepository.GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == classEntity.SubjectId);
                if (subject == null)
                {
                    response.Success = false;
                    response.Message = "Subject not found.";
                    return response;
                }

                // Get all trainee assignments for this class
                var traineeAssignations = await _unitOfWork.TraineeAssignationRepository
                    .GetByNullableExpressionWithOrderingAsync(ta => 
                        ta.ClassTraineeAssignations.Any(cta => cta.ClassId == classId));

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        if (worksheet.Dimension == null)
                        {
                            response.Success = false;
                            response.Message = "Excel file is empty";
                            return response;
                        }

                        // Count actual rows with data (skip empty rows)
                        var dataRowCount = 0;
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var traineeId = worksheet.Cells[row, 1].Text?.Trim();
                            if (!string.IsNullOrEmpty(traineeId))
                            {
                                dataRowCount++;
                            }
                        }

                        result.TotalRows = dataRowCount;

                        // Process each row
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            try
                            {
                                var traineeId = worksheet.Cells[row, 1].Text?.Trim();
                                if (string.IsNullOrEmpty(traineeId))
                                {
                                    continue; // Skip empty rows
                                }

                                // Find trainee assignation
                                var traineeAssignation = traineeAssignations.FirstOrDefault(ta => 
                                    ta.TraineeId == traineeId && 
                                    ta.ClassTraineeAssignations.Any(cta => cta.ClassId == classId));

                                if (traineeAssignation == null)
                                {
                                    result.Errors.Add(new ImportGradeErrorDto
                                    {
                                        RowNumber = row,
                                        TraineeId = traineeId,
                                        Reason = $"Trainee '{traineeId}' is not assigned to this class"
                                    });
                                    result.FailureCount++;
                                    continue;
                                }

                                // Read grade data from Excel
                                var attendanceStr = worksheet.Cells[row, 2].Text?.Trim();
                                var practicalExamStr = worksheet.Cells[row, 3].Text?.Trim();
                                var finalExamStr = worksheet.Cells[row, 4].Text?.Trim();
                                var note = worksheet.Cells[row, 5].Text?.Trim();

                                // Validate required fields based on subject requirements
                                var validationErrors = new List<string>();

                                // Check attendance requirement
                                if (subject.MinAttendance.HasValue)
                                {
                                    if (string.IsNullOrEmpty(attendanceStr))
                                    {
                                        validationErrors.Add("Attendance is required for this subject");
                                    }
                                    else if (!int.TryParse(attendanceStr, out int attendanceVal) || attendanceVal < 0)
                                    {
                                        validationErrors.Add("Invalid attendance value");
                                    }
                                }

                                // Check practical exam requirement
                                if (subject.MinPracticeExamScore.HasValue)
                                {
                                    if (string.IsNullOrEmpty(practicalExamStr))
                                    {
                                        validationErrors.Add("Practical exam score is required for this subject");
                                    }
                                    else if (!decimal.TryParse(practicalExamStr, out decimal practicalScoreVal) || practicalScoreVal < 0 || practicalScoreVal > 10)
                                    {
                                        validationErrors.Add("Invalid practical exam score (must be 0-10)");
                                    }
                                }

                                // Check final exam requirement
                                if (subject.MinFinalExamScore.HasValue)
                                {
                                    if (string.IsNullOrEmpty(finalExamStr))
                                    {
                                        validationErrors.Add("Final exam score is required for this subject");
                                    }
                                    else if (!decimal.TryParse(finalExamStr, out decimal finalScoreVal) || finalScoreVal < 0 || finalScoreVal > 10)
                                    {
                                        validationErrors.Add("Invalid final exam score (must be 0-10)");
                                    }
                                }

                                if (validationErrors.Any())
                                {
                                    result.Errors.Add(new ImportGradeErrorDto
                                    {
                                        RowNumber = row,
                                        TraineeId = traineeId,
                                        Reason = string.Join("; ", validationErrors)
                                    });
                                    result.FailureCount++;
                                    continue;
                                }

                                // Parse values
                                int? attendance = null;
                                decimal? practicalScore = null;
                                decimal? finalScore = null;

                                if (!string.IsNullOrEmpty(attendanceStr) && int.TryParse(attendanceStr, out int att))
                                {
                                    attendance = att;
                                }
                                if (!string.IsNullOrEmpty(practicalExamStr) && decimal.TryParse(practicalExamStr, out decimal prac))
                                {
                                    practicalScore = prac;
                                }
                                if (!string.IsNullOrEmpty(finalExamStr) && decimal.TryParse(finalExamStr, out decimal fin))
                                {
                                    finalScore = fin;
                                }

                                // Delete existing grades for this trainee assignation
                                var existingGrades = await _unitOfWork.TraineeAssignationGradeRepository
                                    .GetByNullableExpressionWithOrderingAsync(g => g.TraineeAssignationId == traineeAssignation.TraineeAssignationId);

                                // Create new grades
                                var grades = new List<TraineeAssignationGrade>();
                                var allPassed = true;

                                // Attendance grade
                                if (attendance.HasValue)
                                {
                                    var attendanceGrade = new TraineeAssignationGrade
                                    {
                                        TraineeAssignationGradeId = Guid.NewGuid().ToString(),
                                        TraineeAssignationId = traineeAssignation.TraineeAssignationId,
                                        GradeKind = Domain.Enums.GradeKind.Attendance,
                                        Grade = attendance.Value,
                                        GradeStatus = subject.MinAttendance.HasValue && attendance.Value >= subject.MinAttendance.Value 
                                            ? Domain.Enums.GradeStatus.Pass 
                                            : Domain.Enums.GradeStatus.Fail,
                                        Note = note,
                                        CreatedAt = DateTime.UtcNow.AddHours(7),
                                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                                    };
                                    grades.Add(attendanceGrade);
                                    if (attendanceGrade.GradeStatus == Domain.Enums.GradeStatus.Fail)
                                        allPassed = false;
                                }

                                // Practical exam grade
                                if (practicalScore.HasValue)
                                {
                                    var practicalGrade = new TraineeAssignationGrade
                                    {
                                        TraineeAssignationGradeId = Guid.NewGuid().ToString(),
                                        TraineeAssignationId = traineeAssignation.TraineeAssignationId,
                                        GradeKind = Domain.Enums.GradeKind.PracticeExamScore,
                                        Grade = practicalScore.Value,
                                        GradeStatus = subject.MinPracticeExamScore.HasValue && practicalScore.Value >= (decimal)subject.MinPracticeExamScore.Value 
                                            ? Domain.Enums.GradeStatus.Pass 
                                            : Domain.Enums.GradeStatus.Fail,
                                        Note = note,
                                        CreatedAt = DateTime.UtcNow.AddHours(7),
                                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                                    };
                                    grades.Add(practicalGrade);
                                    if (practicalGrade.GradeStatus == Domain.Enums.GradeStatus.Fail)
                                        allPassed = false;
                                }

                                // Final exam grade
                                if (finalScore.HasValue)
                                {
                                    var finalGrade = new TraineeAssignationGrade
                                    {
                                        TraineeAssignationGradeId = Guid.NewGuid().ToString(),
                                        TraineeAssignationId = traineeAssignation.TraineeAssignationId,
                                        GradeKind = Domain.Enums.GradeKind.FinalExamScore,
                                        Grade = finalScore.Value,
                                        GradeStatus = subject.MinFinalExamScore.HasValue && finalScore.Value >= (decimal)subject.MinFinalExamScore.Value 
                                            ? Domain.Enums.GradeStatus.Pass 
                                            : Domain.Enums.GradeStatus.Fail,
                                        Note = note,
                                        CreatedAt = DateTime.UtcNow.AddHours(7),
                                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                                    };
                                    grades.Add(finalGrade);
                                    if (finalGrade.GradeStatus == Domain.Enums.GradeStatus.Fail)
                                        allPassed = false;
                                }

                                // Calculate total score if we have both practical and final exam scores
                                if (practicalScore.HasValue && finalScore.HasValue)
                                {
                                    var totalScore = (practicalScore.Value + finalScore.Value) / 2;
                                    var totalGrade = new TraineeAssignationGrade
                                    {
                                        TraineeAssignationGradeId = Guid.NewGuid().ToString(),
                                        TraineeAssignationId = traineeAssignation.TraineeAssignationId,
                                        GradeKind = Domain.Enums.GradeKind.TotalScore,
                                        Grade = totalScore,
                                        GradeStatus = totalScore >= (decimal)subject.MinTotalScore 
                                            ? Domain.Enums.GradeStatus.Pass 
                                            : Domain.Enums.GradeStatus.Fail,
                                        Note = note,
                                        CreatedAt = DateTime.UtcNow.AddHours(7),
                                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                                    };
                                    grades.Add(totalGrade);
                                    if (totalGrade.GradeStatus == Domain.Enums.GradeStatus.Fail)
                                        allPassed = false;
                                }

                                // Save grades to database
                                foreach (var grade in grades)
                                {
                                    await _unitOfWork.TraineeAssignationGradeRepository.AddAsync(grade);
                                }

                                // Update trainee assignation overall status
                                traineeAssignation.OverallGradeStatus = allPassed 
                                    ? Domain.Enums.OverallGradeStatus.Pass 
                                    : Domain.Enums.OverallGradeStatus.Fail;
                                traineeAssignation.GradeDate = DateTime.UtcNow.AddHours(7);
                                traineeAssignation.UpdateDate = DateTime.UtcNow.AddHours(7);

                                await _unitOfWork.TraineeAssignationRepository.UpdateAsync(traineeAssignation);

                                // If trainee passed, check certificate eligibility
                                if (allPassed)
                                {
                                    try
                                    {
                                        await _certificateEligibilityService.ProcessTraineeSubjectCompletionAsync(
                                            traineeAssignation.TraineeId, 
                                            classEntity.SubjectId);
                                    }
                                    catch (Exception ex)
                                    {
                                        // Log error but don't fail the grade import
                                        // You might want to add logging here
                                        Console.WriteLine($"Certificate eligibility check failed: {ex.Message}");
                                    }
                                }

                                result.SuccessCount++;
                            }
                            catch (Exception ex)
                            {
                                result.Errors.Add(new ImportGradeErrorDto
                                {
                                    RowNumber = row,
                                    TraineeId = worksheet.Cells[row, 1].Text?.Trim() ?? "",
                                    Reason = $"Unexpected error: {ex.Message}"
                                });
                                result.FailureCount++;
                            }
                        }
                    }
                }

                // Save all changes
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = result;
                response.Message = $"Grade import completed: {result.SuccessCount} succeeded, {result.FailureCount} failed";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to import grades: {ex.Message}";
                response.Data = result;
            }

            return response;
        }
        #endregion

        #region GetTraineeAssignationsWithGrades
        public async Task<ServiceResponse<List<TraineeAssignationWithGradesDto>>> GetTraineeAssignationsWithGradesAsync(int classId)
        {
            var response = new ServiceResponse<List<TraineeAssignationWithGradesDto>>();
            try
            {
                // Get all trainee assignments for this class
                var traineeAssignations = await _unitOfWork.TraineeAssignationRepository
                    .GetByNullableExpressionWithOrderingAsync(ta => 
                        ta.ClassTraineeAssignations.Any(cta => cta.ClassId == classId));

                var result = new List<TraineeAssignationWithGradesDto>();

                foreach (var ta in traineeAssignations)
                {
                    // Get trainee info
                    var trainee = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == ta.TraineeId);

                    // Get assigned by user info
                    var assignedByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == ta.AssignedByUserId);

                    // Get grades for this trainee assignation
                    var grades = await _unitOfWork.TraineeAssignationGradeRepository
                        .GetByNullableExpressionWithOrderingAsync(g => g.TraineeAssignationId == ta.TraineeAssignationId);

                    var traineeDto = new TraineeAssignationWithGradesDto
                    {
                        TraineeAssignationId = ta.TraineeAssignationId,
                        TraineeId = ta.TraineeId,
                        TraineeName = trainee?.FullName ?? "",
                        AssignedByUserId = ta.AssignedByUserId ?? "",
                        AssignedByUserName = assignedByUser?.FullName ?? "",
                        AssignDate = ta.AssignDate,
                        OverallGradeStatus = ta.OverallGradeStatus,
                        AssignmentKind = ta.AssignmentKind,
                        GradeDate = ta.GradeDate,
                        UpdateDate = ta.UpdateDate,
                        Grades = grades.Select(g => new TraineeAssignationGradeDto
                        {
                            TraineeAssignationGradeId = g.TraineeAssignationGradeId,
                            TraineeAssignationId = g.TraineeAssignationId,
                            GradeKind = g.GradeKind,
                            Grade = g.Grade,
                            GradeStatus = g.GradeStatus,
                            Note = g.Note,
                            CreatedAt = g.CreatedAt,
                            UpdatedAt = g.UpdatedAt
                        }).ToList()
                    };

                    result.Add(traineeDto);
                }

                response.Success = true;
                response.Data = result;
                response.Message = "Trainee assignments with grades retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve trainee assignments: {ex.Message}";
            }

            return response;
        }
        #endregion
    }
}


