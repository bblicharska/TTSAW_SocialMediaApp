using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceApplication.Dto
{
    public class CreateConnectionDto
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
    }
}
