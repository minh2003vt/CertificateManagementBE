using Application.Dto.ClassDto;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClassService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                var instructor = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == dto.InstructorId);
                if (instructor == null)
                {
                    response.Success = false;
                    response.Message = "Instructor not found";
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
    }
}


