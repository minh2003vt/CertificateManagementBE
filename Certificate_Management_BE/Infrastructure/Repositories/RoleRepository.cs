using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(Context context) : base(context)
        {
        }
    }
}


