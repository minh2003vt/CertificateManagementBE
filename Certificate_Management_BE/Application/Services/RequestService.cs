using Application.Dto.RequestDto;
using Application.IServices;
using Application.ServiceResponse;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<RequestDetailDto>> CreateRequestAsync(CreateRequestDto dto, string requestUserId, string entityId, RequestType requestType)
        {
            var response = new ServiceResponse<RequestDetailDto>();
            try
            {
                // Check if entity exists and is pending
                var isEntityPending = await CheckEntityStatus(entityId, requestType);
                if (!isEntityPending)
                {
                    response.Success = false;
                    response.Message = "Entity is not in pending status or does not exist";
                    return response;
                }

                // Generate RequestId
                var lastRequest = await _unitOfWork.RequestRepository
                    .GetByNullableExpressionWithOrderingAsync(null, q => q.OrderByDescending(r => r.RequestId));
                var lastRequestId = lastRequest.FirstOrDefault()?.RequestId ?? "REQ000000";
                var requestIdNumber = int.Parse(lastRequestId.Substring(3)) + 1;
                var newRequestId = $"REQ{requestIdNumber:D6}";

                // Create Request
                var request = new Request
                {
                    RequestId = newRequestId,
                    RequestUserId = requestUserId,
                    Description = dto.Description,
                    Notes = dto.Notes ?? string.Empty,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await _unitOfWork.RequestRepository.AddAsync(request);

                // Create RequestEntity
                var requestEntity = new RequestEntity
                {
                    RequestId = newRequestId,
                    EntityId = entityId,
                    RequestType = requestType,
                    RequestStatus = RequestStatus.Pending
                };

                await _unitOfWork.RequestEntityRepository.AddAsync(requestEntity);
                await _unitOfWork.SaveChangesAsync();

                // Send notification to all Administrators (Directors)
                await NotifyDirectorsAboutNewRequest(newRequestId, requestType, entityId);

                // Load related data for response
                var requestUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == requestUserId);

                var requestDetail = new RequestDetailDto
                {
                    RequestId = request.RequestId,
                    RequestUserId = request.RequestUserId,
                    RequestUserName = requestUser?.FullName ?? "Unknown User",
                    Description = request.Description,
                    Notes = request.Notes,
                    CreatedAt = request.CreatedAt,
                    UpdatedAt = request.UpdatedAt,
                    RequestEntities = new List<RequestEntityDto>
                    {
                        new RequestEntityDto
                        {
                            RequestId = requestEntity.RequestId,
                            EntityId = requestEntity.EntityId,
                            RequestType = requestEntity.RequestType.ToString(),
                            RequestStatus = requestEntity.RequestStatus.ToString()
                        }
                    }
                };

                response.Success = true;
                response.Data = requestDetail;
                response.Message = "Request created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to create request: {ex.Message}";
            }
            return response;
        }

        private async Task<bool> CheckEntityStatus(string entityId, RequestType requestType)
        {
            try
            {
                switch (requestType)
                {
                    case RequestType.NewCourse:
                    case RequestType.ModifyCourse:
                        var course = await _unitOfWork.CourseRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == entityId);
                        return course != null && course.Status == CourseStatus.Pending;

                    case RequestType.newSubject:
                    case RequestType.modifySubject:
                        var subject = await _unitOfWork.SubjectRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == entityId);
                        return subject != null && subject.Status == SubjectStatus.Pending;

                    case RequestType.NewPlan:
                    case RequestType.ModifyPlan:
                        var plan = await _unitOfWork.PlanRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == entityId);
                        return plan != null && plan.Status == PlanStatus.Pending;

                    default:
                        // For CourseSubjectSpecialty, we'll use a different approach
                        var courseSubjectSpecialty = await _unitOfWork.CourseSubjectSpecialtyRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(css => 
                                css.SpecialtyId + css.SubjectId + css.CourseId == entityId);
                        return courseSubjectSpecialty != null && courseSubjectSpecialty.ApprovedAt == null;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<ServiceResponse<List<RequestListDto>>> GetAllRequestsAsync()
        {
            var response = new ServiceResponse<List<RequestListDto>>();
            try
            {
                var requests = await _unitOfWork.RequestRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        null,
                        r => r.OrderByDescending(x => x.CreatedAt)
                    );

                var requestDtos = new List<RequestListDto>();
                foreach (var request in requests)
                {
                    var requestUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == request.RequestUserId);

                    var approvedByUser = request.ApprovedByUserId != null
                        ? await _unitOfWork.UserRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == request.ApprovedByUserId)
                        : null;

                    var requestEntities = await _unitOfWork.RequestEntityRepository
                        .GetByNullableExpressionWithOrderingAsync(re => re.RequestId == request.RequestId, null);

                    requestDtos.Add(new RequestListDto
                    {
                        RequestId = request.RequestId,
                        RequestUserId = request.RequestUserId,
                        RequestUserName = requestUser?.FullName ?? "Unknown User",
                        Description = request.Description,
                        Notes = request.Notes,
                        ApprovedByUserId = request.ApprovedByUserId,
                        ApprovedByUserName = approvedByUser?.FullName,
                        ApprovedDate = request.ApprovedDate,
                        CreatedAt = request.CreatedAt,
                        RequestEntitiesCount = requestEntities.Count
                    });
                }

                response.Success = true;
                response.Data = requestDtos;
                response.Message = "Requests retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve requests: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<RequestDetailDto>> GetRequestByIdAsync(string requestId)
        {
            var response = new ServiceResponse<RequestDetailDto>();
            try
            {
                var request = await _unitOfWork.RequestRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(r => r.RequestId == requestId);

                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Request not found";
                    return response;
                }

                var requestUser = await _unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == request.RequestUserId);

                var approvedByUser = request.ApprovedByUserId != null
                    ? await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == request.ApprovedByUserId)
                    : null;

                var requestEntities = await _unitOfWork.RequestEntityRepository
                    .GetByNullableExpressionWithOrderingAsync(re => re.RequestId == requestId, null);

                var requestDetail = new RequestDetailDto
                {
                    RequestId = request.RequestId,
                    RequestUserId = request.RequestUserId,
                    RequestUserName = requestUser?.FullName ?? "Unknown User",
                    Description = request.Description,
                    Notes = request.Notes,
                    ApprovedByUserId = request.ApprovedByUserId,
                    ApprovedByUserName = approvedByUser?.FullName,
                    ApprovedDate = request.ApprovedDate,
                    CreatedAt = request.CreatedAt,
                    UpdatedAt = request.UpdatedAt,
                    RequestEntities = requestEntities.Select(re => new RequestEntityDto
                    {
                        RequestId = re.RequestId,
                        EntityId = re.EntityId,
                        RequestType = re.RequestType.ToString(),
                        RequestStatus = re.RequestStatus.ToString()
                    }).ToList()
                };

                response.Success = true;
                response.Data = requestDetail;
                response.Message = "Request retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve request: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<List<RequestListDto>>> GetRequestsByUserAsync(string userId)
        {
            var response = new ServiceResponse<List<RequestListDto>>();
            try
            {
                var requests = await _unitOfWork.RequestRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        r => r.RequestUserId == userId,
                        r => r.OrderByDescending(x => x.CreatedAt)
                    );

                var requestDtos = new List<RequestListDto>();
                foreach (var request in requests)
                {
                    var requestUser = await _unitOfWork.UserRepository
                        .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == request.RequestUserId);

                    var approvedByUser = request.ApprovedByUserId != null
                        ? await _unitOfWork.UserRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == request.ApprovedByUserId)
                        : null;

                    var requestEntities = await _unitOfWork.RequestEntityRepository
                        .GetByNullableExpressionWithOrderingAsync(re => re.RequestId == request.RequestId, null);

                    requestDtos.Add(new RequestListDto
                    {
                        RequestId = request.RequestId,
                        RequestUserId = request.RequestUserId,
                        RequestUserName = requestUser?.FullName ?? "Unknown User",
                        Description = request.Description,
                        Notes = request.Notes,
                        ApprovedByUserId = request.ApprovedByUserId,
                        ApprovedByUserName = approvedByUser?.FullName,
                        ApprovedDate = request.ApprovedDate,
                        CreatedAt = request.CreatedAt,
                        RequestEntitiesCount = requestEntities.Count
                    });
                }

                response.Success = true;
                response.Data = requestDtos;
                response.Message = "User requests retrieved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to retrieve user requests: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> ApproveRequestAsync(string requestId, string approvedByUserId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var request = await _unitOfWork.RequestRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(r => r.RequestId == requestId);

                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Request not found";
                    return response;
                }

                // Update request
                request.ApprovedByUserId = approvedByUserId;
                request.ApprovedDate = DateTime.UtcNow.AddHours(7);
                request.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.RequestRepository.UpdateAsync(request);

                // Update request entities and actual entities
                var requestEntities = await _unitOfWork.RequestEntityRepository
                    .GetByNullableExpressionWithOrderingAsync(re => re.RequestId == requestId, null);

                foreach (var requestEntity in requestEntities)
                {
                    requestEntity.RequestStatus = RequestStatus.Approved;
                    await _unitOfWork.RequestEntityRepository.UpdateAsync(requestEntity);

                    // Update the actual entity based on request type
                    await UpdateEntityStatus(requestEntity.EntityId, requestEntity.RequestType, true, approvedByUserId);
                }

                await _unitOfWork.SaveChangesAsync();

                // Send notification to request creator
                await NotifyRequestCreator(request.RequestUserId, "Request Approved", $"Your request has been approved by {approvedByUserId}");

                response.Success = true;
                response.Data = true;
                response.Message = "Request approved successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to approve request: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> RejectRequestAsync(string requestId, string approvedByUserId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var request = await _unitOfWork.RequestRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(r => r.RequestId == requestId);

                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Request not found";
                    return response;
                }

                // Update request
                request.ApprovedByUserId = approvedByUserId;
                request.ApprovedDate = DateTime.UtcNow.AddHours(7);
                request.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.RequestRepository.UpdateAsync(request);

                // Update request entities and actual entities
                var requestEntities = await _unitOfWork.RequestEntityRepository
                    .GetByNullableExpressionWithOrderingAsync(re => re.RequestId == requestId, null);

                foreach (var requestEntity in requestEntities)
                {
                    requestEntity.RequestStatus = RequestStatus.Rejected;
                    await _unitOfWork.RequestEntityRepository.UpdateAsync(requestEntity);

                    // Update the actual entity based on request type
                    await UpdateEntityStatus(requestEntity.EntityId, requestEntity.RequestType, false, approvedByUserId);
                }

                await _unitOfWork.SaveChangesAsync();

                // Send notification to request creator
                await NotifyRequestCreator(request.RequestUserId, "Request Rejected", $"Your request has been rejected by {approvedByUserId}");

                response.Success = true;
                response.Data = true;
                response.Message = "Request rejected successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to reject request: {ex.Message}";
            }
            return response;
        }

        private async Task NotifyDirectorsAboutNewRequest(string requestId, RequestType requestType, string entityId)
        {
            try
            {
                // Get all users with Administrator role (assuming this is Director role)
                var administrators = await _unitOfWork.UserRepository
                    .GetByNullableExpressionWithOrderingAsync(
                        u => u.RoleId == 6, // Assuming RoleId 1 is Administrator
                        null
                    );

                var requestTypeName = requestType.ToString();
                var title = $"New {requestTypeName} Request";
                var message = $"A new {requestTypeName} request has been submitted for entity {entityId}. Request ID: {requestId}";

                foreach (var admin in administrators)
                {
                    var notification = new Notification
                    {
                        UserId = admin.UserId,
                        Title = title,
                        Message = message,
                        NotificationType = "Request",
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        IsRead = false
                    };

                    await _unitOfWork.NotificationRepository.AddAsync(notification);
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request creation
                Console.WriteLine($"Failed to send notifications to directors: {ex.Message}");
            }
        }

        private async Task NotifyRequestCreator(string requestUserId, string title, string message)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = requestUserId,
                    Title = title,
                    Message = message,
                    NotificationType = "Request",
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    IsRead = false
                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't fail the approval/rejection
                Console.WriteLine($"Failed to send notification to request creator: {ex.Message}");
            }
        }

        private async Task UpdateEntityStatus(string entityId, RequestType requestType, bool isApproved, string? approverUserId = null)
        {
            try
            {
                switch (requestType)
                {
                    case RequestType.NewCourse:
                    case RequestType.ModifyCourse:
                        var course = await _unitOfWork.CourseRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(c => c.CourseId == entityId);
                        if (course != null)
                        {
                            course.Status = isApproved ? CourseStatus.Approved : CourseStatus.Rejected;
                            course.UpdatedAt = DateTime.UtcNow.AddHours(7);
                            if (isApproved)
                            {
                                course.AprovedUserId = approverUserId;
                                course.ApprovedAt = DateTime.UtcNow.AddHours(7);
                            }
                            await _unitOfWork.CourseRepository.UpdateAsync(course);
                        }
                        break;

                    case RequestType.newSubject:
                    case RequestType.modifySubject:
                        var subject = await _unitOfWork.SubjectRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(s => s.SubjectId == entityId);
                        if (subject != null)
                        {
                            subject.Status = isApproved ? SubjectStatus.Approved : SubjectStatus.Rejected;
                            subject.UpdatedAt = DateTime.UtcNow.AddHours(7);
                            if (isApproved)
                            {
                                subject.AprovedUserId = approverUserId;
                                subject.ApprovedAt = DateTime.UtcNow.AddHours(7);
                            }
                            await _unitOfWork.SubjectRepository.UpdateAsync(subject);
                        }
                        break;

                    case RequestType.NewPlan:
                    case RequestType.ModifyPlan:
                        var plan = await _unitOfWork.PlanRepository
                            .GetSingleOrDefaultByNullableExpressionAsync(p => p.PlanId == entityId);
                        if (plan != null)
                        {
                            plan.Status = isApproved ? PlanStatus.Approved : PlanStatus.Rejected;
                            plan.UpdatedAt = DateTime.UtcNow.AddHours(7);
                            if (isApproved)
                            {
                                plan.AprovedUserId = approverUserId;
                                plan.ApprovedAt = DateTime.UtcNow.AddHours(7);
                            }
                            await _unitOfWork.PlanRepository.UpdateAsync(plan);
                        }
                        break;

                    case RequestType.NewMatrix:
                    case RequestType.RemoveMatrix:
                        // For CourseSubjectSpecialty, we need to parse the composite key
                        // Format: specialtyId + subjectId + courseId
                        if (entityId.Length >= 3) // Minimum length for composite key
                        {
                            // This is a simplified approach - you might need to adjust based on your ID format
                            var courseSubjectSpecialty = await _unitOfWork.CourseSubjectSpecialtyRepository
                                .GetSingleOrDefaultByNullableExpressionAsync(css => 
                                    css.SpecialtyId + css.SubjectId + css.CourseId == entityId);
                            if (courseSubjectSpecialty != null)
                            {
                                if (isApproved)
                                {
                                    courseSubjectSpecialty.ApprovedAt = DateTime.UtcNow.AddHours(7);
                                    courseSubjectSpecialty.ApprovedByUserId = approverUserId;
                                }
                                else
                                {
                                    // For rejection, you might want to delete the relationship or mark it as rejected
                                    // This depends on your business logic
                                    courseSubjectSpecialty.ApprovedAt = null;
                                }
                                await _unitOfWork.CourseSubjectSpecialtyRepository.UpdateAsync(courseSubjectSpecialty);
                            }
                        }
                        break;

                    default:
                        // Handle other request types if needed
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the approval/rejection
                Console.WriteLine($"Failed to update entity status: {ex.Message}");
            }
        }
    }
}
