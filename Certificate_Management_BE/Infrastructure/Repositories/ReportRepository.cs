using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(Context context) : base(context)
        {
        }
    }
}


