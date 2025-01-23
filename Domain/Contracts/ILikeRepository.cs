using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface ILikeRepository : IRepository<Like>
    {
        int CountLikesForPost(int postId);
        Like GetLike(int postId, int userId);
    }
}
