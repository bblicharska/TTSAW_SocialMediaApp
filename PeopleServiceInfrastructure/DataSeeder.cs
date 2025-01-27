using Microsoft.EntityFrameworkCore;
using PeopleServiceDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceInfrastructure
{
    public class DataSeeder
    {
        private readonly PeopleDbContext _dbContext;

        public DataSeeder(PeopleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                _dbContext.Database.Migrate();

                if (!_dbContext.Connections.Any())
                {
                    var connections = new List<Connection>
                    {
                        new Connection()
                        {UserId=1,
                        FriendId=2,
                         CreatedAt = DateTime.Now.AddDays(-1),
                        },

                    };

                    _dbContext.Connections.AddRange(connections);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
