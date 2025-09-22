using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class TraineeAssignationRepository : GenericRepository<TraineeAssignation>, ITraineeAssignationRepository
    {
        public TraineeAssignationRepository(Context context) : base(context)
        {
        }
    }
}


