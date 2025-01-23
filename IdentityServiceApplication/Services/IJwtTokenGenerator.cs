using IdentityServiceDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServiceApplication.Services
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpirationDate) GenerateToken(User user);
    }

}
