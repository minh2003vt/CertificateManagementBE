using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Application.Dto.UserDto;
using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        #region GetProfile
        public async Task<ServiceResponse<UserProfileDto>> GetUserProfileAsync(string userId)
        {
            var response = new ServiceResponse<UserProfileDto>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                var profile = new UserProfileDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Sex = user.Sex,
                    DateOfBirth = user.DateOfBirth,
                    CitizenId = user.CitizenId,
                };

                response.Success = true;
                response.Data = profile;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve user profile.";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region ImportTrainees
        public async Task<ServiceResponse<ImportResultDto>> ImportTraineesAsync(IFormFile file)
        {
            var response = new ServiceResponse<ImportResultDto>();
            var result = new ImportResultDto();

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Message = "File is empty or not provided";
                    return response;
                }

                // Get all existing data
                var existingUsers = (await _unitOfWork.UserRepository.GetAll()).ToList();
                var specialties = await _unitOfWork.SpecialtyRepository.GetAll();
                var traineeRoleId = 4; // Trainee role

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        // Validate workbook has at least 2 sheets
                        if (package.Workbook.Worksheets.Count < 2)
                        {
                            response.Success = false;
                            response.Message = "Excel file must have at least 2 sheets: Trainees and ExternalCertificates";
                            return response;
                        }

                        var traineeSheet = package.Workbook.Worksheets[0]; 
                        var certSheet = package.Workbook.Worksheets[1]; 

                        // Get total rows with data (excluding header)
                        if (traineeSheet.Dimension == null)
                        {
                            response.Success = false;
                            response.Message = "Trainee sheet is empty";
                            return response;
                        }

                        // Count actual rows with data (skip empty rows)
                        var traineeDataRowCount = 0;
                        for (int row = 2; row <= traineeSheet.Dimension.Rows; row++)
                        {
                            var fullName = traineeSheet.Cells[row, 2].Text?.Trim();
                            if (!string.IsNullOrEmpty(fullName))
                            {
                                traineeDataRowCount++;
                            }
                        }
                        
                        // Limit to maximum 20 records
                        if (traineeDataRowCount > 20)
                        {
                            response.Success = false;
                            response.Message = "Maximum 20 trainees allowed per import. Your file contains " + traineeDataRowCount + " records.";
                            return response;
                        }
                        
                        result.TraineeData.TotalRows = traineeDataRowCount;

                        // Store certificates grouped by CitizenId for later mapping
                        var certificatesByCitizenId = new Dictionary<string, List<ExternalCertificateData>>();
                        
                        // Read images from worksheet.Drawings and map to rows
                        var imagesByRow = new Dictionary<int, MemoryStream>();
                        foreach (var drawing in certSheet.Drawings)
                        {
                            if (drawing is OfficeOpenXml.Drawing.ExcelPicture pic)
                            {
                                // Get row number where image is located (0-based, so add 1)
                                var imageRow = pic.From.Row + 1;
                                
                                var imageBytes = pic.Image.ImageBytes;
                                
                                var imageStream = new MemoryStream(imageBytes);
                                imageStream.Position = 0;
                                
                                imagesByRow[imageRow] = imageStream;
                            }
                        }

                        // Count and read External Certificates sheet
                        var certDataRowCount = 0;
                        if (certSheet.Dimension != null)
                        {
                            for (int row = 2; row <= certSheet.Dimension.Rows; row++)
                            {
                                var citizenId = certSheet.Cells[row, 1].Text?.Trim();
                                if (string.IsNullOrEmpty(citizenId)) continue;

                                certDataRowCount++;

                                var certData = new ExternalCertificateData
                                {
                                    RowNumber = row,
                                    CitizenId = citizenId,
                                    CertificateCode = certSheet.Cells[row, 2].Text?.Trim() ?? "",
                                    CertificateName = certSheet.Cells[row, 3].Text?.Trim() ?? "",
                                    IssuingOrganization = certSheet.Cells[row, 4].Text?.Trim() ?? "",
                                    IssueDate = certSheet.Cells[row, 5].Text?.Trim() ?? "",
                                    ExpiredDate = certSheet.Cells[row, 6].Text?.Trim() ?? "",
                                    ImageStream = imagesByRow.ContainsKey(row) ? imagesByRow[row] : null
                                };

                                if (!certificatesByCitizenId.ContainsKey(citizenId))
                                {
                                    certificatesByCitizenId[citizenId] = new List<ExternalCertificateData>();
                                }
                                certificatesByCitizenId[citizenId].Add(certData);
                            }
                        }
                        result.ExternalCertificateData.TotalRows = certDataRowCount;

                        // Process Trainees
                        for (int row = 2; row <= traineeSheet.Dimension.Rows; row++)
                        {
                            try
                            {
                                // Read data from Excel into DTO
                                var traineeDto = new TraineeImportDto
                                {
                                    RowNumber = row,
                                    UserId = traineeSheet.Cells[row, 1].Text?.Trim(),
                                    FullName = traineeSheet.Cells[row, 2].Text?.Trim() ?? "",
                                    GenderStr = traineeSheet.Cells[row, 3].Text?.Trim() ?? "",
                                    DateOfBirthStr = traineeSheet.Cells[row, 4].Text?.Trim() ?? "",
                                    Address = traineeSheet.Cells[row, 5].Text?.Trim(),
                                    Email = traineeSheet.Cells[row, 6].Text?.Trim() ?? "",
                                    PhoneNumber = traineeSheet.Cells[row, 7].Text?.Trim() ?? "",
                                    CitizenId = traineeSheet.Cells[row, 8].Text?.Trim() ?? "",
                                    SpecialtyId = traineeSheet.Cells[row, 9].Text?.Trim() ?? ""
                                };
                                
                                // Skip empty rows
                                if (string.IsNullOrEmpty(traineeDto.FullName))
                                {
                                    continue;
                                }

                                // Validate using DTO method
                                var validationErrors = traineeDto.Validate(existingUsers, specialties);

                                if (validationErrors.HasErrors)
                                {
                                    result.TraineeData.Errors.Add(new ImportErrorDto
                                    {
                                        RowNumber = row,
                                        Reason = validationErrors.GetErrorMessage(),
                                        FullName = traineeDto.FullName,
                                        Email = traineeDto.Email,
                                        CitizenId = traineeDto.CitizenId
                                    });
                                    result.TraineeData.FailureCount++;
                                    continue;
                                }

                                // Generate UserId if not provided
                                var userId = traineeDto.UserId;
                                if (string.IsNullOrEmpty(userId))
                                {
                                    userId = await GenerateNextUserIdAsync(existingUsers);
                                }

                                // Create User
                                var newUser = new User
                                {
                                    UserId = userId,
                                    Username = traineeDto.Email.Split('@')[0].ToLower(),
                                    FullName = traineeDto.FullName,
                                    Sex = traineeDto.Gender,
                                    DateOfBirth = traineeDto.DateOfBirth,
                                    Address = traineeDto.Address ?? "",
                                    Email = traineeDto.Email,
                                    PhoneNumber = traineeDto.PhoneNumber,
                                    CitizenId = traineeDto.CitizenId,
                                    RoleId = traineeRoleId,
                                    PasswordHash = PasswordHashHelper.HashPassword("VJA@2025"), // Default password
                                    Status = AccountStatus.Pending,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                };

                                await _unitOfWork.UserRepository.AddAsync(newUser);
                                existingUsers.Add(newUser);

                                // Create UserSpecialty
                                var userSpecialty = new UserSpecialty
                                {
                                    UserId = userId,
                                    SpecialtyId = traineeDto.SpecialtyId,
                                    CreatedAt = DateTime.UtcNow
                                };
                                var context = _unitOfWork.Context as DbContext;
                                if (context != null)
                                {
                                    await context.Set<UserSpecialty>().AddAsync(userSpecialty);
                                }

                                result.TraineeData.SuccessCount++;
                            }
                            catch (Exception ex)
                            {
                                result.TraineeData.Errors.Add(new ImportErrorDto
                                {
                                    RowNumber = row,
                                    Reason = $"Unexpected error: {ex.Message}",
                                    FullName = traineeSheet.Cells[row, 2].Text,
                                    Email = traineeSheet.Cells[row, 6].Text
                                });
                                result.TraineeData.FailureCount++;
                            }
                        }

                        var allUsers = (await _unitOfWork.UserRepository.GetAll()).ToList();
                        
                        foreach (var certGroup in certificatesByCitizenId)
                        {
                            var citizenId = certGroup.Key;
                            
                            // Find the user by CitizenId
                            var user = allUsers.FirstOrDefault(u => u.CitizenId == citizenId);
                            if (user == null)
                            {
                                // Add error for each certificate in this group
                                foreach (var certData in certGroup.Value)
                                {
                                    result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                    {
                                        RowNumber = certData.RowNumber,
                                        Reason = $"User with Citizen ID '{citizenId}' not found in database",
                                        CitizenId = citizenId,
                                        CertificateCode = certData.CertificateCode
                                    });
                                    result.ExternalCertificateData.FailureCount++;
                                }
                                continue;
                            }

                            foreach (var certRawData in certGroup.Value)
                            {
                                try
                                {
                                    // Map to DTO
                                    var certDto = new ExternalCertificateImportDto
                                    {
                                        RowNumber = certRawData.RowNumber,
                                        CitizenId = certRawData.CitizenId,
                                        CertificateCode = certRawData.CertificateCode,
                                        CertificateName = certRawData.CertificateName,
                                        IssuingOrganization = certRawData.IssuingOrganization,
                                        IssueDateStr = certRawData.IssueDate,
                                        ExpiredDateStr = certRawData.ExpiredDate,
                                        CertificateImageBase64 = "" 
                                    };

                                    // Validate using DTO method
                                    var certValidation = certDto.Validate();

                                    if (certValidation.HasErrors)
                                    {
                                        result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = certDto.RowNumber,
                                            Reason = certValidation.GetErrorMessage(),
                                            CitizenId = citizenId,
                                            CertificateCode = certDto.CertificateCode
                                        });
                                        result.ExternalCertificateData.FailureCount++;
                                        continue;
                                    }

                                    // Check if image stream exists
                                    if (certRawData.ImageStream == null)
                                    {
                                        result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = certDto.RowNumber,
                                            Reason = "Certificate image is required. No image found in row.",
                                            CitizenId = citizenId,
                                            CertificateCode = certDto.CertificateCode
                                        });
                                        result.ExternalCertificateData.FailureCount++;
                                        continue;
                                    }

                                    // Upload certificate image to Cloudinary
                                    string imageUrl;
                                    try
                                    {
                                        // Reset stream position
                                        certRawData.ImageStream.Position = 0;
                                        
                                        var fileName = $"{citizenId}_{certDto.CertificateCode}_{DateTime.UtcNow.Ticks}.png";
                                        imageUrl = await _cloudinaryService.UploadImageAsync(certRawData.ImageStream, fileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = certDto.RowNumber,
                                            Reason = $"Failed to upload image to Cloudinary: {ex.Message}",
                                            CitizenId = citizenId,
                                            CertificateCode = certDto.CertificateCode
                                        });
                                        result.ExternalCertificateData.FailureCount++;
                                        continue;
                                    }

                                    // Verify imageUrl is valid
                                    if (string.IsNullOrEmpty(imageUrl))
                                    {
                                        result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = certDto.RowNumber,
                                            Reason = "Failed to get image URL from Cloudinary",
                                            CitizenId = citizenId,
                                            CertificateCode = certDto.CertificateCode
                                        });
                                        result.ExternalCertificateData.FailureCount++;
                                        continue;
                                    }

                                    // Create External Certificate with Cloudinary URL
                                    var externalCert = new ExternalCertificate
                                    {
                                        UserId = user.UserId,
                                        CertificateCode = certDto.CertificateCode,
                                        CertificateName = certDto.CertificateName,
                                        IssuingOrganization = certDto.IssuingOrganization,
                                        IssueDate = certDto.IssueDate,
                                        Exp_date = certDto.ExpiredDate,
                                        CertificateFileUrl = imageUrl, // Cloudinary URL
                                        CreatedAt = DateTime.UtcNow
                                    };

                                    await _unitOfWork.ExternalCertificateRepository.AddAsync(externalCert);
                                    result.ExternalCertificateData.SuccessCount++;
                                }
                                catch (Exception ex)
                                {
                                    result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                    {
                                        RowNumber = certRawData.RowNumber,
                                        Reason = $"Unexpected error: {ex.Message}",
                                        CitizenId = citizenId,
                                        CertificateCode = certRawData.CertificateCode
                                    });
                                    result.ExternalCertificateData.FailureCount++;
                                }
                            }
                        }
                    }
                }

                response.Success = true;
                response.Data = result;
                response.Message = $"Trainee Import: {result.TraineeData.SuccessCount} succeeded, {result.TraineeData.FailureCount} failed | Certificate Import: {result.ExternalCertificateData.SuccessCount} succeeded, {result.ExternalCertificateData.FailureCount} failed";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to import trainees: {ex.Message}";
                response.Data = result;
                return response;
            }
        }

        private Task<string> GenerateNextUserIdAsync(List<User> existingUsers)
        {
            var currentYear = DateTime.Now.Year.ToString().Substring(2); // Get last 2 digits of year (e.g., "25")
            var prefix = $"VJA{currentYear}";

            // Find the highest existing number
            var existingNumbers = existingUsers
                .Where(u => u.UserId.StartsWith(prefix))
                .Select(u =>
                {
                    var numberPart = u.UserId.Substring(prefix.Length);
                    return int.TryParse(numberPart, out var num) ? num : 0;
                })
                .ToList();

            var nextNumber = existingNumbers.Any() ? existingNumbers.Max() + 1 : 1;

            // Format: VJA25XXXX (4 digits)
            return Task.FromResult($"{prefix}{nextNumber:D4}");
        }
        #endregion
    }

    // Helper class for external certificate data
    internal class ExternalCertificateData
    {
        public int RowNumber { get; set; }
        public string CitizenId { get; set; } = string.Empty;
        public string CertificateCode { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;
        public string ExpiredDate { get; set; } = string.Empty;
        public MemoryStream? ImageStream { get; set; }
    }
}

