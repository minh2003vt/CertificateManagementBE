using Application.Helpers;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Certificate_Management_BE.Middleware
{
    /// <summary>
    /// Middleware to check approval status before allowing modifications
    /// </summary>
    public class ApprovalStatusMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApprovalStatusMiddleware> _logger;

        public ApprovalStatusMiddleware(RequestDelegate next, ILogger<ApprovalStatusMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only check for PUT, PATCH, DELETE operations
            if (IsModificationRequest(context.Request.Method, context.Request.Path))
            {
                try
                {
                    var entityInfo = ExtractEntityInfo(context.Request.Path);
                    if (entityInfo != null)
                    {
                        var canModify = await CheckEntityApprovalStatus(context, entityInfo);
                        if (!canModify)
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            
                            var errorResponse = new
                            {
                                success = false,
                                message = ApprovalStatusHelper.GetApprovedEntityErrorMessage(entityInfo.EntityType),
                                entityType = entityInfo.EntityType,
                                entityId = entityInfo.EntityId
                            };

                            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking approval status for {Path}", context.Request.Path);
                    // Continue to next middleware if there's an error
                }
            }

            await _next(context);
        }

        private static bool IsModificationRequest(string method, PathString path)
        {
            var pathValue = path.Value?.ToLowerInvariant();
            return method is "PUT" or "PATCH" or "DELETE" && 
                   pathValue != null && 
                   (pathValue.Contains("/subject/") || 
                    pathValue.Contains("/course/") || 
                    pathValue.Contains("/plan/") ||
                    pathValue.Contains("/coursesubjectspecialty/"));
        }

        private static EntityInfo? ExtractEntityInfo(PathString path)
        {
            var pathValue = path.Value?.ToLowerInvariant();
            if (pathValue == null) return null;

            // Extract entity type and ID from path
            if (pathValue.Contains("/subject/"))
            {
                var segments = pathValue.Split('/');
                var subjectIndex = Array.IndexOf(segments, "subject");
                if (subjectIndex >= 0 && subjectIndex + 1 < segments.Length)
                {
                    return new EntityInfo { EntityType = "Subject", EntityId = segments[subjectIndex + 1] };
                }
            }
            else if (pathValue.Contains("/course/"))
            {
                var segments = pathValue.Split('/');
                var courseIndex = Array.IndexOf(segments, "course");
                if (courseIndex >= 0 && courseIndex + 1 < segments.Length)
                {
                    return new EntityInfo { EntityType = "Course", EntityId = segments[courseIndex + 1] };
                }
            }
            else if (pathValue.Contains("/plan/"))
            {
                var segments = pathValue.Split('/');
                var planIndex = Array.IndexOf(segments, "plan");
                if (planIndex >= 0 && planIndex + 1 < segments.Length)
                {
                    return new EntityInfo { EntityType = "Plan", EntityId = segments[planIndex + 1] };
                }
            }

            return null;
        }

        private async Task<bool> CheckEntityApprovalStatus(HttpContext context, EntityInfo entityInfo)
        {
            try
            {
                var unitOfWork = context.RequestServices.GetRequiredService<Application.IUnitOfWork>();

                return entityInfo.EntityType switch
                {
                    "Subject" => await CheckSubjectStatus(unitOfWork, entityInfo.EntityId),
                    "Course" => await CheckCourseStatus(unitOfWork, entityInfo.EntityId),
                    "Plan" => await CheckPlanStatus(unitOfWork, entityInfo.EntityId),
                    _ => true // Default to allowing modification for unknown types
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking approval status for {EntityType} {EntityId}", 
                    entityInfo.EntityType, entityInfo.EntityId);
                return true; // Default to allowing modification on error
            }
        }

        private static async Task<bool> CheckSubjectStatus(Application.IUnitOfWork unitOfWork, string subjectId)
        {
            var subject = await unitOfWork.SubjectRepository
                .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == subjectId);
            
            return subject != null && ApprovalStatusHelper.CanModifySubject(subject);
        }

        private static async Task<bool> CheckCourseStatus(Application.IUnitOfWork unitOfWork, string courseId)
        {
            var course = await unitOfWork.CourseRepository
                .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == courseId);
            
            return course != null && ApprovalStatusHelper.CanModifyCourse(course);
        }

        private static async Task<bool> CheckPlanStatus(Application.IUnitOfWork unitOfWork, string planId)
        {
            var plan = await unitOfWork.PlanRepository
                .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == planId);
            
            return plan != null && ApprovalStatusHelper.CanModifyPlan(plan);
        }

        private class EntityInfo
        {
            public string EntityType { get; set; } = string.Empty;
            public string EntityId { get; set; } = string.Empty;
        }
    }
}





