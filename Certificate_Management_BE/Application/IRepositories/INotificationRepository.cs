using Domain.Entities;

namespace Application.IRepositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetByUserIdAsync(string userId);
    }
}


