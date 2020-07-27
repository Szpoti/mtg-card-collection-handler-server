using MagicApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicApi.Data
{
    public class MTGContext : DbContext
    {
        public MTGContext(DbContextOptions<MTGContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(c => c.Id)
                .HasName("UserId");
        }


    }




}