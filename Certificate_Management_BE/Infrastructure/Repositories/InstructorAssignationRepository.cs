using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class InstructorAssignationRepository : GenericRepository<InstructorAssignation>, IInstructorAssignationRepository
    {
        public InstructorAssignationRepository(Context context) : base(context)
        {
        }
    }
}


