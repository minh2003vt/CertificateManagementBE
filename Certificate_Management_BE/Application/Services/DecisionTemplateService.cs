using Application.Dto.DecisionTemplateDto;
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
    public class DecisionTemplateService : IDecisionTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DecisionTemplateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<DecisionTemplateListDto>>> GetAllAsync()
        {
            var response = new ServiceResponse<List<DecisionTemplateListDto>>();
            try
            {
                var templates = await _unitOfWork.DecisionTemplateRepository.GetAll();
                var templateList = new List<DecisionTemplateListDto>();

                foreach (var template in templates)
                {
                    var createdByUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);

                    templateList.Add(new DecisionTemplateListDto
                    {
                        DecisionTemplateId = template.DecisionTemplateId,
                        TemplateName = template.TemplateName,
                        Description = template.Description,
                        TemplateStatus = template.TemplateStatus,
                        CreatedByUserName = createdByUser?.FullName ?? ""
                    });
                }

                response.Success = true;
                response.Data = templateList;
                response.Message = $"Retrieved {templateList.Count} decision templates";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve decision templates: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<DecisionTemplateDetailDto>> GetByIdAsync(string templateId)
        {
            var response = new ServiceResponse<DecisionTemplateDetailDto>();
            try
            {
                var template = await _unitOfWork.DecisionTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(dt => dt.DecisionTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Decision template not found.";
                    return response;
                }

                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);
                var approvedByUser = template.ApprovedByUserId != null 
                    ? await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.ApprovedByUserId)
                    : null;

                var detail = new DecisionTemplateDetailDto
                {
                    DecisionTemplateId = template.DecisionTemplateId,
                    TemplateName = template.TemplateName,
                    Description = template.Description,
                    TemplateFileUrl = template.TemplateFileUrl,
                    CreatedByUserId = template.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName ?? "",
                    TemplateStatus = template.TemplateStatus,
                    ApprovedByUserId = template.ApprovedByUserId,
                    ApprovedByUserName = approvedByUser?.FullName ?? "",
                    CreatedAt = template.CreatedAt,
                    LastUpdatedAt = template.LastUpdatedAt
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Decision template retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve decision template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<DecisionTemplateDetailDto>> CreateAsync(CreateDecisionTemplateDto dto, string createdByUserId)
        {
            var response = new ServiceResponse<DecisionTemplateDetailDto>();
            try
            {
                var template = new DecisionTemplate
                {
                    DecisionTemplateId = Guid.NewGuid().ToString(),
                    TemplateName = dto.TemplateName,
                    Description = dto.Description,
                    TemplateFileUrl = null, // No file initially
                    CreatedByUserId = createdByUserId,
                    TemplateStatus = TemplateStatus.Inactive,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    LastUpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.DecisionTemplateRepository.AddAsync(template);
                await _unitOfWork.SaveChangesAsync();

                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == createdByUserId);

                var detail = new DecisionTemplateDetailDto
                {
                    DecisionTemplateId = template.DecisionTemplateId,
                    TemplateName = template.TemplateName,
                    Description = template.Description,
                    TemplateFileUrl = template.TemplateFileUrl,
                    CreatedByUserId = template.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName ?? "",
                    TemplateStatus = template.TemplateStatus,
                    ApprovedByUserId = template.ApprovedByUserId,
                    ApprovedByUserName = "",
                    CreatedAt = template.CreatedAt,
                    LastUpdatedAt = template.LastUpdatedAt
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Decision template created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create decision template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<DecisionTemplateDetailDto>> ImportTemplateAsync(string templateId, IFormFile templateFile)
        {
            var response = new ServiceResponse<DecisionTemplateDetailDto>();
            try
            {
                var template = await _unitOfWork.DecisionTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(dt => dt.DecisionTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Decision template not found.";
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

                await _unitOfWork.DecisionTemplateRepository.UpdateAsync(template);
                await _unitOfWork.SaveChangesAsync();

                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);
                var approvedByUser = template.ApprovedByUserId != null 
                    ? await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.ApprovedByUserId)
                    : null;

                var detail = new DecisionTemplateDetailDto
                {
                    DecisionTemplateId = template.DecisionTemplateId,
                    TemplateName = template.TemplateName,
                    Description = template.Description,
                    TemplateFileUrl = template.TemplateFileUrl,
                    CreatedByUserId = template.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName ?? "",
                    TemplateStatus = template.TemplateStatus,
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

        public async Task<ServiceResponse<DecisionTemplateDetailDto>> UpdateAsync(string templateId, UpdateDecisionTemplateDto dto, string updatedByUserId)
        {
            var response = new ServiceResponse<DecisionTemplateDetailDto>();
            try
            {
                var template = await _unitOfWork.DecisionTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(dt => dt.DecisionTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Decision template not found.";
                    return response;
                }

                template.TemplateName = dto.TemplateName;
                template.Description = dto.Description;
                template.TemplateStatus = dto.TemplateStatus;
                template.LastUpdatedAt = DateTime.UtcNow.AddHours(7);

                // If setting to Active, set all other decision templates to Inactive
                if (dto.TemplateStatus == TemplateStatus.Active)
                {
                    var otherTemplates = await _unitOfWork.DecisionTemplateRepository
                        .GetByNullableExpressionWithOrderingAsync(dt => dt.DecisionTemplateId != templateId);

                    foreach (var otherTemplate in otherTemplates)
                    {
                        otherTemplate.TemplateStatus = TemplateStatus.Inactive;
                        otherTemplate.LastUpdatedAt = DateTime.UtcNow.AddHours(7);
                        await _unitOfWork.DecisionTemplateRepository.UpdateAsync(otherTemplate);
                    }
                }

                await _unitOfWork.DecisionTemplateRepository.UpdateAsync(template);
                await _unitOfWork.SaveChangesAsync();

                var createdByUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.CreatedByUserId);
                var approvedByUser = template.ApprovedByUserId != null 
                    ? await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == template.ApprovedByUserId)
                    : null;

                var detail = new DecisionTemplateDetailDto
                {
                    DecisionTemplateId = template.DecisionTemplateId,
                    TemplateName = template.TemplateName,
                    Description = template.Description,
                    TemplateFileUrl = template.TemplateFileUrl,
                    CreatedByUserId = template.CreatedByUserId,
                    CreatedByUserName = createdByUser?.FullName ?? "",
                    TemplateStatus = template.TemplateStatus,
                    ApprovedByUserId = template.ApprovedByUserId,
                    ApprovedByUserName = approvedByUser?.FullName ?? "",
                    CreatedAt = template.CreatedAt,
                    LastUpdatedAt = template.LastUpdatedAt
                };

                response.Success = true;
                response.Data = detail;
                response.Message = "Decision template updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to update decision template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(string templateId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var template = await _unitOfWork.DecisionTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(dt => dt.DecisionTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Decision template not found.";
                    return response;
                }

                // Check if template is being used by any decisions
                var decisionsUsingTemplate = await _unitOfWork.DecisionRepository
                    .GetByNullableExpressionWithOrderingAsync(d => d.DecisionTemplateId == templateId);

                if (decisionsUsingTemplate.Any())
                {
                    response.Success = false;
                    response.Message = "Cannot delete decision template that is being used by decisions.";
                    return response;
                }

                await _unitOfWork.DecisionTemplateRepository.DeleteAsync(template);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Decision template deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to delete decision template: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ApproveAsync(string templateId, string approvedByUserId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var template = await _unitOfWork.DecisionTemplateRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(dt => dt.DecisionTemplateId == templateId);

                if (template == null)
                {
                    response.Success = false;
                    response.Message = "Decision template not found.";
                    return response;
                }

                // Set all other decision templates to Inactive (only one can be active at a time)
                var otherTemplates = await _unitOfWork.DecisionTemplateRepository
                    .GetByNullableExpressionWithOrderingAsync(dt => dt.DecisionTemplateId != templateId);

                foreach (var otherTemplate in otherTemplates)
                {
                    otherTemplate.TemplateStatus = TemplateStatus.Inactive;
                    otherTemplate.LastUpdatedAt = DateTime.UtcNow.AddHours(7);
                    await _unitOfWork.DecisionTemplateRepository.UpdateAsync(otherTemplate);
                }

                // Set current template to Active
                template.TemplateStatus = TemplateStatus.Active;
                template.ApprovedByUserId = approvedByUserId;
                template.LastUpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.DecisionTemplateRepository.UpdateAsync(template);
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = $"Decision template approved successfully. {otherTemplates.Count()} other decision templates have been set to inactive.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to approve decision template: {ex.Message}";
            }

            return response;
        }
    }
}
