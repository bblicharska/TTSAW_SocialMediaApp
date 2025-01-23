using PeopleServiceApplication.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceApplication.Services
{
    public interface IConnectionService
    {
        Task<ConnectionDto> GetConnectionAsync(int id);
        Task<bool> IsTokenValidAsync(string token);  
        Task<IEnumerable<ConnectionDto>> GetUserConnectionsAsync(int userId);
        Task<ConnectionDto> CreateConnectionAsync(CreateConnectionDto dto);
        Task<bool> DeleteConnectionAsync(int id);
        Task<ConnectionDto> UpdateConnectionAsync(int id, UpdateConnectionDto dto);
    }
}
