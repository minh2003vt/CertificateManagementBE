using Application;
using Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IUserRepository _userRepository;
        public UnitOfWork(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }
        public IUserRepository UserRepository => _userRepository;
    }
}
