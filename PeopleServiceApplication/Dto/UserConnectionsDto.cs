using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceApplication.Dto
{
    public class UserConnectionsDto
    {
        public int UserId { get; set; }
        public List<ConnectionDto> Connections { get; set; }
    }
}
