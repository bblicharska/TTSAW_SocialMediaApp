using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class DataSeeder
    {
        private readonly PostDbContext _dbContext;

        public DataSeeder(PostDbContext context)
        {
            this._dbContext = context;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                _dbContext.Database.Migrate();

                if (!_dbContext.Posts.Any())
                {
                    var posts = new List<Post>
                    {
                        new Post()
                        {
                            UserId = 1,
                            Content = "Hej widzowie. Pozdrowionka z Podgoricy :)",
                            CreatedAt =  DateTime.Now.AddDays(-3),
                            ImageUrl = "/images/hotel1.jpg",
                            Comments = new List<Comment>
                            {
                                new Comment
                                {
                                    UserId = 2,
                                    Content = "Wow, super miejsce! Jak wrażenia?",
                                    CreatedAt = DateTime.Now.AddDays(-2)
                                },
                            },
                            Likes = new List<Like>
                            {
                                new Like
                                {
                                UserId = 2,
                                CreatedAt = DateTime.Now.AddDays(-2)
                                },
                            }
                        },

                        new Post()
                        {
                            UserId = 1,
                            Content = "Hej widzowie. Pozdrowionka z Bangkoku :)",
                            CreatedAt =  DateTime.Now.AddDays(-1),
                            ImageUrl = "/images/hotel2.jpg",
                        },
                    };

                    _dbContext.Posts.AddRange(posts);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
