using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
   public class Like
    {
        public int Id { get; set; } 
        public int PostId { get; set; } 
        public int UserId { get; set; } 
        public DateTime CreatedAt { get; set; } 

        public Post Post { get; set; }
    }

}
