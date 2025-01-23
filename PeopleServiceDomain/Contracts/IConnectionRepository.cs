using PeopleServiceDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceDomain.Contracts
{
    public interface IConnectionRepository
    {
        Task<Connection> GetByIdAsync(int id);
        Task<IEnumerable<Connection>> GetByUserIdAsync(int userId);
        Task AddAsync(Connection connection);
        Task DeleteAsync(Connection connection);
        Task UpdateAsync(Connection connection);
    }
}
