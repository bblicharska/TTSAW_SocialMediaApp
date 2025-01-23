using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class PostUnitOfWork : IPostUnitOfWork
    {
        private readonly PostDbContext _context;

        public IPostRepository PostRepository { get; }
        public ICommentRepository CommentRepository { get; }
        public ILikeRepository LikeRepository { get; }


        public PostUnitOfWork(PostDbContext context, IPostRepository postRepository, ICommentRepository commentRepository, ILikeRepository likeRepository)
        {
            _context = context;
            this.PostRepository = postRepository;
            this.CommentRepository= commentRepository;
            this.LikeRepository = likeRepository;
        }
        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
