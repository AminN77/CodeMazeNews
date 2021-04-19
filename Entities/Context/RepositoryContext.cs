using Entities.Configuration;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities.Context
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new NewsConfiguration());
        }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}