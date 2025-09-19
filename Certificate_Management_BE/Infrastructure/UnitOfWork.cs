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
        private readonly ISessionRepository _sessionRepository;
        public UnitOfWork(IUserRepository userRepository, ISessionRepository sessionRepository) 
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }
        public IUserRepository UserRepository => _userRepository;
        public ISessionRepository SessionRepository => _sessionRepository;
    }
}
