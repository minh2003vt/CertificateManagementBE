using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class TraineeAssignationGradeRepository : GenericRepository<TraineeAssignationGrade>, ITraineeAssignationGradeRepository
    {
        public TraineeAssignationGradeRepository(Infrastructure.Context context) : base(context)
        {
        }
    }
}
