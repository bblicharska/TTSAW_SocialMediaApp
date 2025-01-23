using Microsoft.EntityFrameworkCore;
using PeopleServiceDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PeopleServiceInfrastructure
{
    public class PeopleDbContext : DbContext
    {
        public DbSet<Connection> Connections { get; set; }

        public PeopleDbContext(DbContextOptions<PeopleDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Connection>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Connection>()
                .HasIndex(c => new { c.UserId, c.FriendId })
                .IsUnique();
        }
    }
}
