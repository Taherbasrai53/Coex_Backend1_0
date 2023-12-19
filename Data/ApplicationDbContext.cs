using COeX_India1._0.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;

namespace COeX_India1._0.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Cluster> Clusters { get; set; }
        public DbSet<Mine> Mines { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}
