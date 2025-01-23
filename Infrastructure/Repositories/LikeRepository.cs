using Domain.Contracts;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class LikeRepository :Repository<Like>, ILikeRepository
    {
        private readonly PostDbContext _context;

        public LikeRepository(PostDbContext context):base(context)
        {
            _context = context;
        }

        public int CountLikesForPost(int postId)
        {
            // Zlicza liczbę polubień dla danego postu
            return _context.Likes
                .Count(l => l.PostId == postId);
        }

        public Like GetLike(int postId, int userId)
        {
            // Sprawdza, czy istnieje polubienie dla danego postu przez określonego użytkownika
            return _context.Likes
                .FirstOrDefault(l => l.PostId == postId && l.UserId == userId);
        }

        public Like Get(int id)
        {
            return _context.Likes
                .FirstOrDefault(l => l.Id == id);
        }

        public IEnumerable<Like> GetAll()
        {
            return _context.Likes.ToList();
        }
    }

}
