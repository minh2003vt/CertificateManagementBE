using Application.IRepositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(Context context) : base(context)
        {
        }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            var user = await _context.Users
    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.Trim().ToLower());
            return user;
        }
    }
}
