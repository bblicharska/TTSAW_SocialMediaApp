using IdentityServiceDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceDomain.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByUsernameOrEmail(string usernameOrEmail);
        User GetById(int userId);
        bool UserExists(string email);
        void SaveChanges();
    }
}
