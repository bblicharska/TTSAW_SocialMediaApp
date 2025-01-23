using Domain.Contracts;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly PostDbContext _postDbContext;

        public PostRepository(PostDbContext context)
            : base(context)
        {
            _postDbContext = context;
        }

        public Post Get(int id)
        {
            return _postDbContext.Posts
                .FirstOrDefault(p => p.Id == id);
        }

        public IQueryable<Post> GetAll()
        {
            return _postDbContext.Posts
         .Include(p => p.Comments)
         .Include(p => p.Likes);
        }

        public Post GetById(int id)
        {
            return _postDbContext.Posts
         .Include(p => p.Comments)
         .Include(p => p.Likes)
         .Where(p => p.Id == id)
         .FirstOrDefault();
        }
    }
}
