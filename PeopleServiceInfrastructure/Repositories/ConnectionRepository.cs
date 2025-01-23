using Microsoft.EntityFrameworkCore;
using PeopleServiceDomain.Contracts;
using PeopleServiceDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceInfrastructure.Repositories
{
    public class ConnectionRepository : IConnectionRepository
    {
        private readonly PeopleDbContext _context;

        public ConnectionRepository(PeopleDbContext context)
        {
            _context = context;
        }

        public async Task<Connection> GetByIdAsync(int id)
        {
            return await _context.Connections.FindAsync(id);
        }

        public async Task<IEnumerable<Connection>> GetByUserIdAsync(int userId)
        {
            return await _context.Connections.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task AddAsync(Connection connection)
        {
            await _context.Connections.AddAsync(connection);
        }

        public async Task DeleteAsync(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task UpdateAsync(Connection connection)
        {
            _context.Connections.Update(connection);
        }
    }
}
