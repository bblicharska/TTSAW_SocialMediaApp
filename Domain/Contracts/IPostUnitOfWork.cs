using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IPostUnitOfWork : IDisposable
    {
        IPostRepository PostRepository { get; }
        ICommentRepository CommentRepository { get; }
        ILikeRepository LikeRepository { get; }

        void Commit();
    }
}
