using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class SpecialtyRepository : GenericRepository<Specialty>, ISpecialtyRepository
    {
        public SpecialtyRepository(Context context) : base(context)
        {
        }
    }
}


