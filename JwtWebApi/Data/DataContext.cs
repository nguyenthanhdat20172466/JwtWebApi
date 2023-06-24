using JwtWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtWebApi.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<RoleUser> Roles { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<CountedBy> CountedBy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Counter>()
                .HasOne(c => c.Character)
                .WithMany(c => c.Counters)
                .HasForeignKey(c => c.CharacterId);

            modelBuilder.Entity<CountedBy>()
                .HasOne(c => c.Character)
                .WithMany(c => c.CountedBy)
                .HasForeignKey(c => c.CharacterId);
        }

    }
}
