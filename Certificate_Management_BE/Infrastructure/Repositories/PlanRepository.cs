using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class PlanRepository : GenericRepository<Plan>, IPlanRepository
    {
        public PlanRepository(Context context) : base(context)
        {
        }
    }
}