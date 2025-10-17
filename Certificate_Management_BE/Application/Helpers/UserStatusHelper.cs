using Application.IRepositories;
using Domain.Enums;
using Domain.Entities;

namespace Application.Helpers
{
    public static class UserStatusHelper
    {
        /// <summary>
        /// Check if a user has an active account status
        /// </summary>
        /// <param name="unitOfWork">Unit of work for database access</param>
        /// <param name="userId">User ID to check</param>
        /// <returns>True if user is active, false otherwise</returns>
        public static async Task<bool> IsUserActiveAsync(IUnitOfWork unitOfWork, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            try
            {
                var user = await unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                return user?.Status == AccountStatus.Active;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get user account status
        /// </summary>
        /// <param name="unitOfWork">Unit of work for database access</param>
        /// <param name="userId">User ID to check</param>
        /// <returns>User's account status, or null if not found</returns>
        public static async Task<AccountStatus?> GetUserStatusAsync(IUnitOfWork unitOfWork, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            try
            {
                var user = await unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                return user?.Status;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Validate that a user is active, throw exception if not
        /// </summary>
        /// <param name="unitOfWork">Unit of work for database access</param>
        /// <param name="userId">User ID to validate</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if user is not active</exception>
        public static async Task ValidateUserIsActiveAsync(IUnitOfWork unitOfWork, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var user = await unitOfWork.UserRepository
                .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            if (user.Status != AccountStatus.Active)
                throw new UnauthorizedAccessException($"User account is not active. Status: {user.Status}");
        }

        /// <summary>
        /// Get user with status validation
        /// </summary>
        /// <param name="unitOfWork">Unit of work for database access</param>
        /// <param name="userId">User ID to get</param>
        /// <returns>User entity if active, null otherwise</returns>
        public static async Task<User?> GetActiveUserAsync(IUnitOfWork unitOfWork, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            try
            {
                var user = await unitOfWork.UserRepository
                    .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                return user?.Status == AccountStatus.Active ? user : null;
            }
            catch
            {
                return null;
            }
        }
    }
}





