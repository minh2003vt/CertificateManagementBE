using Application.Dto.RequestDto;
using Application.IServices;
using Certificate_Management_BE.Attributes;
using Certificate_Management_BE.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificate_Management_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        /// <summary>
        /// Create a new request (generic endpoint - use specific entity endpoints instead)
        /// </summary>
        [HttpPost("{entityId}/{requestType}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> CreateRequest(string entityId, string requestType, [FromBody] CreateRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            if (!Enum.TryParse<Domain.Enums.RequestType>(requestType, out var parsedRequestType))
            {
                return BadRequest("Invalid request type");
            }

            var result = await _requestService.CreateRequestAsync(dto, userId, entityId, parsedRequestType);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all requests (Administrator and Education Officer only)
        /// </summary>
        [HttpGet]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> GetAllRequests()
        {
            var result = await _requestService.GetAllRequestsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get request by ID
        /// </summary>
        [HttpGet("{requestId}")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetRequestById(string requestId)
        {
            var result = await _requestService.GetRequestByIdAsync(requestId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get requests by current user
        /// </summary>
        [HttpGet("my-requests")]
        [AuthorizeRoles("Administrator", "Education Officer", "Instructor", "Trainee")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var result = await _requestService.GetRequestsByUserAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Approve a request (Administrator and Education Officer only)
        /// </summary>
        [HttpPost("{requestId}/approve")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> ApproveRequest(string requestId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var result = await _requestService.ApproveRequestAsync(requestId, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Reject a request (Administrator and Education Officer only)
        /// </summary>
        [HttpPost("{requestId}/reject")]
        [AuthorizeRoles("Administrator", "Education Officer")]
        public async Task<IActionResult> RejectRequest(string requestId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var result = await _requestService.RejectRequestAsync(requestId, userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
