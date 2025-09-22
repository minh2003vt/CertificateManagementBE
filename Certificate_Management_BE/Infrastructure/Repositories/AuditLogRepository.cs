using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(Context context) : base(context)
        {
        }
    }
}


