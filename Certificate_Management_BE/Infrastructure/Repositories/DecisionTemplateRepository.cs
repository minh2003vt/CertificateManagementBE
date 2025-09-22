using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class DecisionTemplateRepository : GenericRepository<DecisionTemplate>, IDecisionTemplateRepository
    {
        public DecisionTemplateRepository(Context context) : base(context)
        {
        }
    }
}


