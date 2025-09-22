using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(Context context) : base(context)
        {
        }
    }
}


