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
        public DbSet<Format> Formats { get; private set; }
        public DbSet<Deck> Decks { get; private set; }
        public DbSet<DeckCards> DeckCards { get; private set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(c => c.Id)
                .HasName("UserId");
            modelBuilder.Entity<User>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Format>().HasKey(c => c.Id);
            modelBuilder.Entity<Format>().Property(c => c.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Deck>().HasKey(c => c.Id);
            modelBuilder.Entity<Deck>().Property(c => c.Id).ValueGeneratedOnAdd();

        }
    }
}