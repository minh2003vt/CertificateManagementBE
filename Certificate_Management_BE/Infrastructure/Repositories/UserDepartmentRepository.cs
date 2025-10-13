using Application.IRepositories;
using Domain.Entities;
using Infrastructure;

namespace Infrastructure.Repositories
{
    public class UserDepartmentRepository : GenericRepository<UserDepartment>, IUserDepartmentRepository
    {
        public UserDepartmentRepository(Context context) : base(context)
        {
        }
    }
}
