using Application.Dto.CertificateTemplateDto;
using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CertificateTemplateService : ICertificateTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CertificateTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<CertificateTemplateListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<CertificateTemplateListDto>>();
            try
            {
                var templates = await _unitOfWork.CertificateTemplateRepository.GetAll();
                var templateList = new List<CertificateTemplateListDto>();

                foreach (var template in templates)
                {
                    var createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);

                    templateList.Add(new CertificateTemplateListDto
                    {
                        CertificateTemplateId = template.CertificateTemplateId,
                        TemplateName = template.TemplateName,
                        Description = template.Description,
                        TemplateStatus = template.TemplateStatus,
                        CertificateKind = template.CertificateKind,
                        CreatedByUserName = createdByUser?.FullName ?? ""
                    });
                }

                response.Success = true;
                response.Data = templateList;
                response.Message = $"Retrieved {templateList.Count} certificate templates";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve certificate templates: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CertificateTemplateDetailDto>> GetByIdAsync(string templateId)
        {
            var response = new ServiceResponse<CertificateTemplateDetailDto>();
            try
            {
                var template = await _unitOfWork.CertificateTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(ct => ct.CertificateTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Certificate template not found.";
                    return response;
                }

                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);
                var approvedByUser = template.ApprovedByUserId != null 
                    ? await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.ApprovedByUserId)
                    : null;

                var detail = new CertificateTemplateDetailDto
                {
                    CertificateTemplateId = template.CertificateTemplateId,
                    TemplateName = template.TemplateName,
                    Description = template.Description,
                    TemplateFileUrl = template.TemplateFileUrl,
                    CreatedByUserId = template.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName ?? "",
                    TemplateStatus = template.TemplateStatus,
                    CertificateKind = template.CertificateKind,
                    ApprovedByUserId = template.ApprovedByUserId,
                    ApprovedByUserName = approvedByUser?.FullName ?? "",
                    CreatedAt = template.CreatedAt,
                    LastUpdatedAt = template.LastUpdatedAt
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Certificate template retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve certificate template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CertificateTemplateDetailDto>> CreateAsync(CreateCertificateTemplateDto dto, string createdByUserId)
        {
            var response = new ServiceResponse<CertificateTemplateDetailDto>();
            try
            {
                var template = new CertificateTemplate
                {
                    CertificateTemplateId = Guid.NewGuid().ToString(),
                    TemplateName = dto.TemplateName,
                    Description = dto.Description,
                    TemplateFileUrl = null, // No file initially
                    CreatedByUserId = createdByUserId,
                    TemplateStatus = TemplateStatus.Inactive,
                    CertificateKind = dto.CertificateKind,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    LastUpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.CertificateTemplateRepository.AddAsync(template);
                await _unitOfWork.SaveChangesAsync();

                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == createdByUserId);

                var detail = new CertificateTemplateDetailDto
                {
                    CertificateTemplateId = template.CertificateTemplateId,
                    TemplateName = template.TemplateName,
                    Description = template.Description,
                    TemplateFileUrl = template.TemplateFileUrl,
                    CreatedByUserId = template.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName ?? "",
                    TemplateStatus = template.TemplateStatus,
                    CertificateKind = template.CertificateKind,
                    ApprovedByUserId = template.ApprovedByUserId,
                    ApprovedByUserName = "",
                    CreatedAt = template.CreatedAt,
                    LastUpdatedAt = template.LastUpdatedAt
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Certificate template created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create certificate template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CertificateTemplateDetailDto>> ImportTemplateAsync(string templateId, IFormFile templateFile)
        {
            var response = new ServiceResponse<CertificateTemplateDetailDto>();
            try
            {
                var template = await _unitOfWork.CertificateTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(ct => ct.CertificateTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Certificate template not found.";
                    return response;
                }

                // Validate file
                if (templateFile == null || templateFile.Length == 0)
                {
                    response.Success = false;
                    response.Message = "Template file is required.";
                    return response;
                }

                if (!templateFile.ContentType.Contains("html") && !templateFile.FileName.EndsWith(".html"))
                {
                    response.Success = false;
                    response.Message = "Only HTML files are allowed.";
                    return response;
                }

                // Read HTML content from file
                string htmlContent;
                using (var stream = new MemoryStream())
                {
                    await templateFile.CopyToAsync(stream);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                    {
                        htmlContent = await reader.ReadToEndAsync();
                    }
                }

                template.TemplateFileUrl = htmlContent;
                template.LastUpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.CertificateTemplateRepository.UpdateAsync(template);
                await _unitOfWork.SaveChangesAsync();

                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);
                var approvedByUser = template.ApprovedByUserId != null 
                    ? await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.ApprovedByUserId)
                    : null;

                var detail = new CertificateTemplateDetailDto
                {
                    CertificateTemplateId = template.CertificateTemplateId,
                    TemplateName = template.TemplateName,
                    Description = template.Description,
                    TemplateFileUrl = template.TemplateFileUrl,
                    CreatedByUserId = template.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName ?? "",
                    TemplateStatus = template.TemplateStatus,
                    CertificateKind = template.CertificateKind,
                    ApprovedByUserId = template.ApprovedByUserId,
                    ApprovedByUserName = approvedByUser?.FullName ?? "",
                    CreatedAt = template.CreatedAt,
                    LastUpdatedAt = template.LastUpdatedAt
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Template file imported successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to import template file: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CertificateTemplateDetailDto>> UpdateAsync(string templateId, UpdateCertificateTemplateDto dto, string updatedByUserId)
        {
            var response = new ServiceResponse<CertificateTemplateDetailDto>();
            try
            {
                var template = await _unitOfWork.CertificateTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(ct => ct.CertificateTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Certificate template not found.";
                    return response;
                }

                template.TemplateName = dto.TemplateName;
                template.Description = dto.Description;
                template.CertificateKind = dto.CertificateKind;
                template.TemplateStatus = dto.TemplateStatus;
                template.LastUpdatedAt = DateTime.UtcNow.AddHours(7);

                // If setting to Active, set all other templates of the same CertificateKind to Inactive
                if (dto.TemplateStatus == TemplateStatus.Active)
                {
                    var otherTemplates = await _unitOfWork.CertificateTemplateRepository
                        .GetByNullableExpressionWithOrderingAsync(ct => 
                            ct.CertificateKind == template.CertificateKind && 
                            ct.CertificateTemplateId != templateId);

                    foreach (var otherTemplate in otherTemplates)
                    {
                        otherTemplate.TemplateStatus = TemplateStatus.Inactive;
                        otherTemplate.LastUpdatedAt = DateTime.UtcNow.AddHours(7);
                        await _unitOfWork.CertificateTemplateRepository.UpdateAsync(otherTemplate);
                    }
                }

                await _unitOfWork.CertificateTemplateRepository.UpdateAsync(template);
                await _unitOfWork.SaveChangesAsync();

                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);
                var approvedByUser = template.ApprovedByUserId != null 
                    ? await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.ApprovedByUserId)
                    : null;

                var detail = new CertificateTemplateDetailDto
                {
                    CertificateTemplateId = template.CertificateTemplateId,
                    TemplateName = template.TemplateName,
                    Description = template.Description,
                    TemplateFileUrl = template.TemplateFileUrl,
                    CreatedByUserId = template.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName ?? "",
                    TemplateStatus = template.TemplateStatus,
                    CertificateKind = template.CertificateKind,
                    ApprovedByUserId = template.ApprovedByUserId,
                    ApprovedByUserName = approvedByUser?.FullName ?? "",
                    CreatedAt = template.CreatedAt,
                    LastUpdatedAt = template.LastUpdatedAt
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Certificate template updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update certificate template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(string templateId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var template = await _unitOfWork.CertificateTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(ct => ct.CertificateTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Certificate template not found.";
                    return response;
                }

                // Check if template is being used by any certificates
                var certificatesUsingTemplate = await _unitOfWork.CertificateRepository
                    .GetByNullableExpressionWithOrderingAsync(c => c.CertificateTemplateId == templateId);

                if (certificatesUsingTemplate.Any())
                {
                    response.Success = false;
                    response.Message = "Cannot delete certificate template that is being used by certificates.";
                    return response;
                }

                await _unitOfWork.CertificateTemplateRepository.DeleteAsync(template);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Certificate template deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete certificate template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ApproveAsync(string templateId, string approvedByUserId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var template = await _unitOfWork.CertificateTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(ct => ct.CertificateTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Certificate template not found.";
                    return response;
                }

                // Set all other templates of the same CertificateKind to Inactive
                var otherTemplates = await _unitOfWork.CertificateTemplateRepository
                    .GetByNullableExpressionWithOrderingAsync(ct => 
                        ct.CertificateKind == template.CertificateKind && 
                        ct.CertificateTemplateId != templateId);

                foreach (var otherTemplate in otherTemplates)
                {
                    otherTemplate.TemplateStatus = TemplateStatus.Inactive;
                    otherTemplate.LastUpdatedAt = DateTime.UtcNow.AddHours(7);
                    await _unitOfWork.CertificateTemplateRepository.UpdateAsync(otherTemplate);
                }

                // Set current template to Active
                template.TemplateStatus = TemplateStatus.Active;
                template.ApprovedByUserId = approvedByUserId;
                template.LastUpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.CertificateTemplateRepository.UpdateAsync(template);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = $"Certificate template approved successfully. {otherTemplates.Count()} other templates of the same kind have been set to inactive.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to approve certificate template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<CertificateTemplateListDto>>> GetByCertificateKindAsync(CertificateKind certificateKind)
        {
            var response = new ServiceResponse<List<CertificateTemplateListDto>>();
            try
            {
                var templates = await _unitOfWork.CertificateTemplateRepository
                    .GetByNullableExpressionWithOrderingAsync(ct => ct.CertificateKind == certificateKind);

                var templateList = new List<CertificateTemplateListDto>();

                foreach (var template in templates)
                {
                    var createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);

                    templateList.Add(new CertificateTemplateListDto
                    {
                        CertificateTemplateId = template.CertificateTemplateId,
                        TemplateName = template.TemplateName,
                        Description = template.Description,
                        TemplateStatus = template.TemplateStatus,
                        CertificateKind = template.CertificateKind,
                        CreatedByUserName = createdByUser?.FullName ?? ""
                    });
                }

                response.Success = true;
                response.Data = templateList;
                response.Message = $"Retrieved {templateList.Count} certificate templates for kind {certificateKind}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve certificate templates by kind: {ex.Message}";
            }

            return response;
        }
    }
}
