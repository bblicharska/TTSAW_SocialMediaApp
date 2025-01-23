using Domain.Contracts;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly PostDbContext _context;

        public CommentRepository(PostDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Comment> GetCommentsForPost(int postId)
        {
            // Pobiera wszystkie komentarze, które są powiązane z danym postem
            return _context.Comments
                .Where(c => c.PostId == postId)
                .ToList();
        }

        public Comment Get(int id)
        {
            return _context.Comments
                .FirstOrDefault(c => c.Id == id);
        }
        public IEnumerable<Comment> GetAll()
        {
            return _context.Comments.ToList();
        }
        public int Count()
        {
            return _context.Comments.Count();
        }
    }

}
