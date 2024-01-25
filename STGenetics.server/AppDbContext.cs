using Microsoft.EntityFrameworkCore;
using STGenetics.Shared;

namespace STGenetics.Server
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Animal> Animal{ get; set; }

    }
}
