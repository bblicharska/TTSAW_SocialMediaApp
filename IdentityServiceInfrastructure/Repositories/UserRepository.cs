using IdentityServiceDomain.Contracts;
using IdentityServiceDomain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceInfrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public User Get(int id)
        {
            return _context.Users.Find(id);
        }

        public IList<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public void Insert(User user)
        {
            _context.Users.Add(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public IList<User> Find(Expression<Func<User, bool>> predicate)
        {
            return _context.Users.Where(predicate).ToList();
        }

        public User GetByUsernameOrEmail(string usernameOrEmail)
        {
            return _context.Users
                .FirstOrDefault(user => user.Username == usernameOrEmail || user.Email == usernameOrEmail);
        }

        public User GetById(int userId)
        {
            return _context.Users.Find(userId);
        }

        public bool UserExists(string email)
        {
            return _context.Users.Any(user => user.Email == email);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }

}
