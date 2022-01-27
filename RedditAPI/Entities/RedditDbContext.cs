using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditAPI.Entities
{
    public class RedditDbContext : DbContext
    {
        public DbSet<History> DrawHistory { get; set; }

        public RedditDbContext(DbContextOptions<RedditDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<History>()
                .HasIndex(e => e.DrawDate);
        }
    }
}
