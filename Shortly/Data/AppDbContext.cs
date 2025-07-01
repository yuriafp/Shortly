using Microsoft.EntityFrameworkCore;
using Shortly.API.Models;

namespace Shortly.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UrlMapping> UrlMappings => Set<UrlMapping>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlMapping>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.ShortCode)
                .IsUnique();

                //entity.HasQueryFilter(x => !x.IsDeleted);
            });
        }
    }
}
