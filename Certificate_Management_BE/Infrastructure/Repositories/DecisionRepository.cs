using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class DecisionRepository : GenericRepository<Decision>, IDecisionRepository
    {
        public DecisionRepository(Context context) : base(context)
        {
        }
    }
}


