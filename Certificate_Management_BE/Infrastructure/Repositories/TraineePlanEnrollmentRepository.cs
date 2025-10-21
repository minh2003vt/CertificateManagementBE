using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class TraineePlanEnrollmentRepository : GenericRepository<TraineePlanEnrollment>, ITraineePlanEnrollmentRepository
    {
        public TraineePlanEnrollmentRepository(Infrastructure.Context context) : base(context)
        {
        }
    }
}
