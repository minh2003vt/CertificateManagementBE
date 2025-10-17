using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class ClassGroupRepository : GenericRepository<ClassGroup>, IClassGroupRepository
    {
        public ClassGroupRepository(Context context) : base(context)
        {
        }
    }
}


