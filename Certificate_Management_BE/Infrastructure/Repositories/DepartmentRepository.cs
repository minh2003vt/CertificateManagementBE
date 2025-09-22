using Application.IRepositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(Context context) : base(context)
        {
        }
    }
}


