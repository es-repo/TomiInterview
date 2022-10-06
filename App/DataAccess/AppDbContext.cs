using App.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace App.DataAccess
{
    public class AppDbContext : DbContext
    {
        public string ConnectionString { get; }

        public DbSet<Company> Companies { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        public AppDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }
        }
    }
}
