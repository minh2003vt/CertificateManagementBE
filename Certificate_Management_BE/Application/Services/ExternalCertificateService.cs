using Application.Dto.ExternalCertificateDto;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ExternalCertificateService : IExternalCertificateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public ExternalCertificateService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        #region GetAll
        public async Task<ServiceResponse<List<ExternalCertificateListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<ExternalCertificateListDto>>();
            try
            {
                var certificates = await _unitOfWork.ExternalCertificateRepository.GetAll();
                var certificateList = certificates.Select(c => new ExternalCertificateListDto
                {
                    ExternalCertificateId = c.ExternalCertificateId,
                    CertificateCode = c.CertificateCode,
                    CertificateName = c.CertificateName
                }).ToList();

                response.Success = true;
                response.Data = certificateList;
                response.Message = $"Retrieved {certificateList.Count} certificates";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve certificates";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region GetAllByUserId
        public async Task<ServiceResponse<List<ExternalCertificateListDto>>> GetAllByUserIdAsync(string userId)
        {
            var response = new ServiceResponse<List<ExternalCertificateListDto>>();
            try
            {
                // Check if user exists
                var user = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = $"User with ID '{userId}' not found";
                    return response;
                }

                var certificates = await _unitOfWork.ExternalCertificateRepository.GetAll();
                var userCertificates = certificates
                    .Where(c => c.UserId == userId)
                    .Select(c => new ExternalCertificateListDto
                    {
                        ExternalCertificateId = c.ExternalCertificateId,
                        CertificateCode = c.CertificateCode,
                        CertificateName = c.CertificateName,
                    })
                    .ToList();

                response.Success = true;
                response.Data = userCertificates;
                response.Message = $"Retrieved {userCertificates.Count} certificates for user '{user.FullName}'";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve certificates";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region GetById
        public async Task<ServiceResponse<ExternalCertificateDetailDto>> GetByIdAsync(int id)
        {
            var response = new ServiceResponse<ExternalCertificateDetailDto>();
            try
            {
                var certificate = await _unitOfWork.ExternalCertificateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.ExternalCertificateId == id);

                if (certificate == null)
                {
                    response.Success = false;
                    response.Message = "Certificate not found";
                    return response;
                }

                // Load User navigation property if needed
                if (certificate.User == null && !string.IsNullOrEmpty(certificate.UserId))
                {
                    var user = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == certificate.UserId);
                    certificate.User = user;
                }

                var detailDto = new ExternalCertificateDetailDto
                {
                    ExternalCertificateId = certificate.ExternalCertificateId,
                    CertificateCode = certificate.CertificateCode,
                    CertificateName = certificate.CertificateName,
                    IssuingOrganization = certificate.IssuingOrganization,
                    UserId = certificate.UserId,
                    UserFullName = certificate.User?.FullName,
                    IssueDate = certificate.IssueDate,
                    Exp_date = certificate.Exp_date,
                    CertificateFileUrl = certificate.CertificateFileUrl,
                    CreatedAt = certificate.CreatedAt
                };

                response.Success = true;
                response.Data = detailDto;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve certificate";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region Create
        public async Task<ServiceResponse<ExternalCertificateDetailDto>> CreateAsync(string userId, CreateExternalCertificateDto dto)
        {
            var response = new ServiceResponse<ExternalCertificateDetailDto>();
            try
            {
                // Validate using DTO
                var validationErrors = dto.Validate();
                if (validationErrors.HasErrors)
                {
                    response.Success = false;
                    response.Message = validationErrors.GetErrorMessage();
                    return response;
                }

                // Check if user exists (from query parameter)
                var user = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = $"User with ID '{userId}' not found";
                    return response;
                }

                // Upload file to Cloudinary
                string imageUrl;
                try
                {
                    using (var stream = dto.CertificateFile.OpenReadStream())
                    {
                        var fileName = $"{userId}_{dto.CertificateCode}_{DateTime.UtcNow.Ticks}{System.IO.Path.GetExtension(dto.CertificateFile.FileName)}";
                        imageUrl = await _cloudinaryService.UploadImageAsync(stream, fileName);
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"Failed to upload certificate image: {ex.Message}";
                    return response;
                }

                // Ensure dates are UTC
                var issueDate = DateTime.SpecifyKind(dto.IssueDate, DateTimeKind.Utc);
                var expDate = DateTime.SpecifyKind(dto.Exp_date, DateTimeKind.Utc);

                // Create certificate
                var certificate = new ExternalCertificate
                {
                    CertificateCode = dto.CertificateCode,
                    CertificateName = dto.CertificateName,
                    IssuingOrganization = dto.IssuingOrganization,
                    UserId = userId,
                    IssueDate = issueDate,
                    Exp_date = expDate,
                    CertificateFileUrl = imageUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ExternalCertificateRepository.AddAsync(certificate);

                // Return detail
                var detailDto = new ExternalCertificateDetailDto
                {
                    ExternalCertificateId = certificate.ExternalCertificateId,
                    CertificateCode = certificate.CertificateCode,
                    CertificateName = certificate.CertificateName,
                    IssuingOrganization = certificate.IssuingOrganization,
                    UserId = certificate.UserId,
                    UserFullName = user.FullName,
                    IssueDate = certificate.IssueDate,
                    Exp_date = certificate.Exp_date,
                    CertificateFileUrl = certificate.CertificateFileUrl,
                    CreatedAt = certificate.CreatedAt
                };

                response.Success = true;
                response.Data = detailDto;
                response.Message = "Certificate created successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to create certificate";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region Update
        public async Task<ServiceResponse<ExternalCertificateDetailDto>> UpdateAsync(int id, UpdateExternalCertificateDto dto)
        {
            var response = new ServiceResponse<ExternalCertificateDetailDto>();
            try
            {
                // Validate using DTO
                var validationErrors = dto.Validate();
                if (validationErrors.HasErrors)
                {
                    response.Success = false;
                    response.Message = validationErrors.GetErrorMessage();
                    return response;
                }

                // Check if certificate exists
                var certificate = await _unitOfWork.ExternalCertificateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.ExternalCertificateId == id);

                if (certificate == null)
                {
                    response.Success = false;
                    response.Message = "Certificate not found";
                    return response;
                }

                // Get user
                var user = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == certificate.UserId);

                // Ensure dates are UTC
                var issueDate = DateTime.SpecifyKind(dto.IssueDate, DateTimeKind.Utc);
                var expDate = DateTime.SpecifyKind(dto.Exp_date, DateTimeKind.Utc);

                // Update certificate (NOT changing UserId or CertificateFileUrl)
                certificate.CertificateCode = dto.CertificateCode;
                certificate.CertificateName = dto.CertificateName;
                certificate.IssuingOrganization = dto.IssuingOrganization;
                certificate.IssueDate = issueDate;
                certificate.Exp_date = expDate;

                await _unitOfWork.ExternalCertificateRepository.UpdateAsync(certificate);

                // Return detail
                var detailDto = new ExternalCertificateDetailDto
                {
                    ExternalCertificateId = certificate.ExternalCertificateId,
                    CertificateCode = certificate.CertificateCode,
                    CertificateName = certificate.CertificateName,
                    IssuingOrganization = certificate.IssuingOrganization,
                    UserId = certificate.UserId,
                    UserFullName = user?.FullName,
                    IssueDate = certificate.IssueDate,
                    Exp_date = certificate.Exp_date,
                    CertificateFileUrl = certificate.CertificateFileUrl,
                    CreatedAt = certificate.CreatedAt
                };

                response.Success = true;
                response.Data = detailDto;
                response.Message = "Certificate updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update certificate";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region UpdateCertificateFile
        public async Task<ServiceResponse<ExternalCertificateDetailDto>> UpdateCertificateFileAsync(int id, Microsoft.AspNetCore.Http.IFormFile file)
        {
            var response = new ServiceResponse<ExternalCertificateDetailDto>();
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Message = "Certificate file is required";
                    return response;
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    response.Success = false;
                    response.Message = "Only JPG, JPEG, and PNG files are allowed";
                    return response;
                }

                // Max 10MB
                if (file.Length > 10 * 1024 * 1024)
                {
                    response.Success = false;
                    response.Message = "File size must not exceed 10MB";
                    return response;
                }

                // Check if certificate exists
                var certificate = await _unitOfWork.ExternalCertificateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.ExternalCertificateId == id);

                if (certificate == null)
                {
                    response.Success = false;
                    response.Message = "Certificate not found";
                    return response;
                }

                // Upload file to Cloudinary
                string imageUrl;
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var fileName = $"{certificate.UserId}_{certificate.CertificateCode}_{DateTime.UtcNow.Ticks}{extension}";
                        imageUrl = await _cloudinaryService.UploadImageAsync(stream, fileName);
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"Failed to upload certificate image: {ex.Message}";
                    return response;
                }

                // Update certificate file URL
                certificate.CertificateFileUrl = imageUrl;
                await _unitOfWork.ExternalCertificateRepository.UpdateAsync(certificate);

                // Get user
                var user = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == certificate.UserId);

                // Return detail
                var detailDto = new ExternalCertificateDetailDto
                {
                    ExternalCertificateId = certificate.ExternalCertificateId,
                    CertificateCode = certificate.CertificateCode,
                    CertificateName = certificate.CertificateName,
                    IssuingOrganization = certificate.IssuingOrganization,
                    UserId = certificate.UserId,
                    UserFullName = user?.FullName,
                    IssueDate = certificate.IssueDate,
                    Exp_date = certificate.Exp_date,
                    CertificateFileUrl = certificate.CertificateFileUrl,
                    CreatedAt = certificate.CreatedAt
                };

                response.Success = true;
                response.Data = detailDto;
                response.Message = "Certificate file updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update certificate file";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region Delete
        public async Task<ServiceResponse<string>> DeleteAsync(int id)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var certificate = await _unitOfWork.ExternalCertificateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(c => c.ExternalCertificateId == id);

                if (certificate == null)
                {
                    response.Success = false;
                    response.Message = "Certificate not found";
                    return response;
                }

                await _unitOfWork.ExternalCertificateRepository.DeleteAsync(certificate);

                response.Success = true;
                response.Data = "Certificate deleted";
                response.Message = "Certificate deleted successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to delete certificate";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion
    }
}

