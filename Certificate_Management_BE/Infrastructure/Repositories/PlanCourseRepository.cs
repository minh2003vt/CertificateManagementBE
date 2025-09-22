using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class PlanCourseRepository : GenericRepository<PlanCourse>, IPlanCourseRepository
    {
        public PlanCourseRepository(Context context) : base(context)
        {
        }
    }
}


