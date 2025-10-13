using Application.Dto.SpecialtyDto;
using Application.IServices;
using Application;
using Application.ServiceResponse;
using Domain.Entities;

namespace Application.Services
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecialtyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<SpecialtyListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<SpecialtyListDto>>();
            try
            {
                var specialties = await _unitOfWork.SpecialtyRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        null,
                        s => s.OrderBy(x => x.CreatedAt)
                    );

                var specialtyDtos = specialties.Select(s => new SpecialtyListDto
                {
                    SpecialtyId = s.SpecialtyId,
                    SpecialtyName = s.SpecialtyName,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt
                }).ToList();

                response.Success = true;
                response.Data = specialtyDtos;
                response.Message = $"Retrieved {specialtyDtos.Count} specialties successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve specialties: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<SpecialtyDetailDto>> GetByIdAsync(string specialtyId)
        {
            var response = new ServiceResponse<SpecialtyDetailDto>();
            try
            {
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == specialtyId);

                if (specialty == null)
                {
                    response.Success = false;
                    response.Message = "Specialty not found";
                    return response;
                }

                // Load CreatedByUser navigation property
                if (!string.IsNullOrEmpty(specialty.CreatedByUserId))
                {
                    var createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == specialty.CreatedByUserId);
                    specialty.CreatedByUser = createdByUser;
                }

                // Load UpdatedByUser navigation property
                if (!string.IsNullOrEmpty(specialty.UpdatedByUserId))
                {
                    var updatedByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == specialty.UpdatedByUserId);
                    specialty.UpdatedByUser = updatedByUser;
                }

                var specialtyDto = new SpecialtyDetailDto
                {
                    SpecialtyId = specialty.SpecialtyId,
                    SpecialtyName = specialty.SpecialtyName,
                    Description = specialty.Description,
                    CreatedByUserId = specialty.CreatedByUserId,
                    CreatedByUserName = specialty.CreatedByUser?.FullName,
                    UpdatedByUserId = specialty.UpdatedByUserId,
                    UpdatedByUserName = specialty.UpdatedByUser?.FullName,
                    CreatedAt = specialty.CreatedAt,
                    UpdatedAt = specialty.UpdatedAt
                };

                response.Success = true;
                response.Data = specialtyDto;
                response.Message = "Specialty retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve specialty: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<SpecialtyDetailDto>> CreateAsync(CreateSpecialtyDto dto, string createdByUserId)
        {
            var response = new ServiceResponse<SpecialtyDetailDto>();
            try
            {
                // Validate DTO
                var validationErrors = dto.Validate();
                if (validationErrors.HasErrors)
                {
                    response.Success = false;
                    response.Message = "Validation failed";
                    response.ErrorMessages = validationErrors.Errors;
                    return response;
                }

                // Check if specialty ID already exists
                var existingSpecialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == dto.SpecialtyId);

                if (existingSpecialty != null)
                {
                    response.Success = false;
                    response.Message = $"Specialty with ID '{dto.SpecialtyId}' already exists";
                    return response;
                }

                // Create new specialty
                var specialty = new Specialty
                {
                    SpecialtyId = dto.SpecialtyId,
                    SpecialtyName = dto.SpecialtyName,
                    Description = dto.Description ?? string.Empty,
                    CreatedByUserId = createdByUserId,
                    UpdatedByUserId = createdByUserId,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.SpecialtyRepository.AddAsync(specialty);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve the created specialty with user info
                var createdSpecialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == specialty.SpecialtyId);

                // Load CreatedByUser
                if (!string.IsNullOrEmpty(createdSpecialty.CreatedByUserId))
                {
                    var user = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == createdSpecialty.CreatedByUserId);
                    createdSpecialty.CreatedByUser = user;
                }

                var specialtyDto = new SpecialtyDetailDto
                {
                    SpecialtyId = createdSpecialty.SpecialtyId,
                    SpecialtyName = createdSpecialty.SpecialtyName,
                    Description = createdSpecialty.Description,
                    CreatedByUserId = createdSpecialty.CreatedByUserId,
                    CreatedByUserName = createdSpecialty.CreatedByUser?.FullName,
                    UpdatedByUserId = createdSpecialty.UpdatedByUserId,
                    UpdatedByUserName = createdSpecialty.CreatedByUser?.FullName,
                    CreatedAt = createdSpecialty.CreatedAt,
                    UpdatedAt = createdSpecialty.UpdatedAt
                };

                response.Success = true;
                response.Data = specialtyDto;
                response.Message = "Specialty created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create specialty: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<SpecialtyDetailDto>> UpdateAsync(string specialtyId, UpdateSpecialtyDto dto, string updatedByUserId)
        {
            var response = new ServiceResponse<SpecialtyDetailDto>();
            try
            {
                // Validate DTO
                var validationErrors = dto.Validate();
                if (validationErrors.HasErrors)
                {
                    response.Success = false;
                    response.Message = "Validation failed";
                    response.ErrorMessages = validationErrors.Errors;
                    return response;
                }

                // Get existing specialty
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == specialtyId);

                if (specialty == null)
                {
                    response.Success = false;
                    response.Message = "Specialty not found";
                    return response;
                }

                // Update specialty
                specialty.SpecialtyName = dto.SpecialtyName;
                specialty.Description = dto.Description ?? string.Empty;
                specialty.UpdatedByUserId = updatedByUserId;
                specialty.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SpecialtyRepository.UpdateAsync(specialty);
                await _unitOfWork.SaveChangesAsync();

                // Retrieve updated specialty with user info
                var updatedSpecialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == specialtyId);

                // Load CreatedByUser
                if (!string.IsNullOrEmpty(updatedSpecialty.CreatedByUserId))
                {
                    var createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == updatedSpecialty.CreatedByUserId);
                    updatedSpecialty.CreatedByUser = createdByUser;
                }

                // Load UpdatedByUser
                if (!string.IsNullOrEmpty(updatedSpecialty.UpdatedByUserId))
                {
                    var updatedByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == updatedSpecialty.UpdatedByUserId);
                    updatedSpecialty.UpdatedByUser = updatedByUser;
                }

                var specialtyDto = new SpecialtyDetailDto
                {
                    SpecialtyId = updatedSpecialty.SpecialtyId,
                    SpecialtyName = updatedSpecialty.SpecialtyName,
                    Description = updatedSpecialty.Description,
                    CreatedByUserId = updatedSpecialty.CreatedByUserId,
                    CreatedByUserName = updatedSpecialty.CreatedByUser?.FullName,
                    UpdatedByUserId = updatedSpecialty.UpdatedByUserId,
                    UpdatedByUserName = updatedSpecialty.UpdatedByUser?.FullName,
                    CreatedAt = updatedSpecialty.CreatedAt,
                    UpdatedAt = updatedSpecialty.UpdatedAt
                };

                response.Success = true;
                response.Data = specialtyDto;
                response.Message = "Specialty updated successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update specialty: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(string specialtyId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var specialty = await _unitOfWork.SpecialtyRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(s => s.SpecialtyId == specialtyId);

                if (specialty == null)
                {
                    response.Success = false;
                    response.Message = "Specialty not found";
                    return response;
                }

                await _unitOfWork.SpecialtyRepository.DeleteAsync(specialty);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Specialty deleted successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete specialty: {ex.Message}";
            }

            return response;
        }
    }
}

