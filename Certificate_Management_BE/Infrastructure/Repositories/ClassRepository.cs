using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        public ClassRepository(Context context) : base(context)
        {
        }
    }
}


