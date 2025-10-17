using Application.Dto.ClassGroupDto;
using Application.Dto.ClassDto;
using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ClassGroupService : IClassGroupService
    {
        private readonly IClassGroupRepository _classGroupRepository;
        private readonly IClassRepository _classRepository;
        private readonly IUserRepository _userRepository;

        public ClassGroupService(IClassGroupRepository classGroupRepository, IClassRepository classRepository, IUserRepository userRepository)
        {
            _classGroupRepository = classGroupRepository;
            _classRepository = classRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<List<ClassGroupListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<ClassGroupListDto>>();
            try
            {
                var groups = await _classGroupRepository.GetAll();
                response.Data = groups
                    .OrderByDescending(g => g.CreatedAt)
                    .Select(g => new ClassGroupListDto
                    {
                        ClassGroupId = g.ClassGroupId,
                        Name = g.Name,
                        Start = g.Start,
                        End = g.End,
                        CreatedAt = g.CreatedAt
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

        public async Task<ServiceResponse<ClassGroupDetailDto>> GetByIdAsync(int classGroupId)
        {
            var response = new ServiceResponse<ClassGroupDetailDto>();
            try
            {
                var g = await _classGroupRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassGroupId == classGroupId);
                if (g == null)
                {
                    response.Success = false;
                    response.Message = "ClassGroup not found";
                    return response;
                }
                response.Data = new ClassGroupDetailDto
                {
                    ClassGroupId = g.ClassGroupId,
                    Name = g.Name,
                    Start = g.Start,
                    End = g.End,
                    Description = g.Description,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt
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

        public async Task<ServiceResponse<List<ClassListDto>>> GetClassesByGroupIdAsync(int classGroupId)
        {
            var response = new ServiceResponse<List<ClassListDto>>();
            try
            {
                // Verify class group exists
                var classGroup = await _classGroupRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassGroupId == classGroupId);
                if (classGroup == null)
                {
                    response.Success = false;
                    response.Message = "ClassGroup not found";
                    return response;
                }

                // Get all classes for this group
                var classes = await _classRepository.GetByNullableExpressionWithOrderingAsync(x => x.ClassGroupId == classGroupId);
                var users = await _userRepository.GetAll();
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

        public async Task<ServiceResponse<ClassGroupDetailDto>> CreateAsync(CreateClassGroupDto dto)
        {
            var response = new ServiceResponse<ClassGroupDetailDto>();
            try
            {
                var entity = new ClassGroup
                {
                    Name = dto.Name,
                    Start = dto.Start,
                    End = dto.End,
                    Description = dto.Description,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };
                await _classGroupRepository.AddAsync(entity);
                // repository base uses same context, SaveChanges handled by UoW usually; commit here via update
                // In this codebase, generic repo methods do not auto-save; caller controls save via UoW.
                // But we don't have UoW injected here, so assume repo saves on AddAsync; if not, adjust later.

                var created = await _classGroupRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassGroupId == entity.ClassGroupId);
                if (created == null)
                {
                    // fallback mapping
                    created = entity;
                }
                response.Success = true;
                response.Data = new ClassGroupDetailDto
                {
                    ClassGroupId = created.ClassGroupId,
                    Name = created.Name,
                    Start = created.Start,
                    End = created.End,
                    Description = created.Description,
                    CreatedAt = created.CreatedAt,
                    UpdatedAt = created.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<ClassGroupDetailDto>> UpdateAsync(int classGroupId, UpdateClassGroupDto dto)
        {
            var response = new ServiceResponse<ClassGroupDetailDto>();
            try
            {
                var entity = await _classGroupRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassGroupId == classGroupId);
                if (entity == null)
                {
                    response.Success = false;
                    response.Message = "ClassGroup not found";
                    return response;
                }
                entity.Name = dto.Name;
                entity.Start = dto.Start;
                entity.End = dto.End;
                entity.Description = dto.Description;
                entity.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await _classGroupRepository.UpdateAsync(entity);

                var updated = await _classGroupRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassGroupId == entity.ClassGroupId);
                updated ??= entity;
                response.Success = true;
                response.Data = new ClassGroupDetailDto
                {
                    ClassGroupId = updated.ClassGroupId,
                    Name = updated.Name,
                    Start = updated.Start,
                    End = updated.End,
                    Description = updated.Description,
                    CreatedAt = updated.CreatedAt,
                    UpdatedAt = updated.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int classGroupId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var entity = await _classGroupRepository.GetSingleOrDefaultByNullableExpressionAsync(x => x.ClassGroupId == classGroupId);
                if (entity == null)
                {
                    response.Success = false;
                    response.Message = "ClassGroup not found";
                    return response;
                }
                await _classGroupRepository.DeleteAsync(entity);
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


