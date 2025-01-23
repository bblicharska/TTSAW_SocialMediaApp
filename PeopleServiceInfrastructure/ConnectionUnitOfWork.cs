using PeopleServiceDomain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceInfrastructure
{
    public class ConnectionUnitOfWork : IConnectionUnitOfWork
    {
        private readonly PeopleDbContext _context;

        public IConnectionRepository ConnectionRepository { get; }


        public ConnectionUnitOfWork(PeopleDbContext context, IConnectionRepository connectionRepository)
        {
            _context = context;
            this.ConnectionRepository = connectionRepository;
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
