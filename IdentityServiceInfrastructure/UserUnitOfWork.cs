using IdentityServiceDomain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceInfrastructure
{
    public class UserUnitOfWork : IUserUnitOfWork
    {
        private readonly UserDbContext _context;

        public IUserRepository UserRepository { get; }


        public UserUnitOfWork(UserDbContext context, IUserRepository userRepository)
        {
            _context = context;
            this.UserRepository = userRepository;
        }
        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
