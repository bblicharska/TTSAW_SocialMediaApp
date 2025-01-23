using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICommentService
    {
        int CreateComment(int postId, CreateCommentDto dto);
        void DeleteComment(int commentId);
        List<CommentDto> GetCommentsByPostId(int postId);
    }

}
