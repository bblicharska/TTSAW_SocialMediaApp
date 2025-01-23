using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ILikeService
    {
        void AddLike(int postId, CreateLikeDto dto);
        void RemoveLike(int likeId);
        List<LikeDto> GetLikesByPostId(int postId);
        int CountLikesByPostId(int postId);
        bool IsPostLikedByUser(int postId, int userId);
    }

}
