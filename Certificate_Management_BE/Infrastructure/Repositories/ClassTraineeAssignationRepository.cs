using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class ClassTraineeAssignationRepository : GenericRepository<ClassTraineeAssignation>, IClassTraineeAssignationRepository
    {
        public ClassTraineeAssignationRepository(Context context) : base(context)
        {
        }
    }
}


