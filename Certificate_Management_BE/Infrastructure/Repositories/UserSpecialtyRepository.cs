using Application.IRepositories;
using Domain.Entities;
using Infrastructure;

namespace Infrastructure.Repositories
{
    public class UserSpecialtyRepository : GenericRepository<UserSpecialty>, IUserSpecialtyRepository
    {
        public UserSpecialtyRepository(Context context) : base(context)
        {
        }
    }
}
