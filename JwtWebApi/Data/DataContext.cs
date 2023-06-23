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
    }
}
