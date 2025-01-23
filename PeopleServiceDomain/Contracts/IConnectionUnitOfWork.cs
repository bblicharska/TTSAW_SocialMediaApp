using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceDomain.Contracts
{
    public interface IConnectionUnitOfWork : IDisposable
    {
        IConnectionRepository ConnectionRepository { get; }

        void Commit();
    }
}
