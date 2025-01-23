using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceDomain.Contracts
{
    public interface IUserUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }

        void Commit();
    }
}