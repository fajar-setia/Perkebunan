using Microsoft.EntityFrameworkCore;
using Perkebunan.Models;
using Perkebunan.Data;

namespace Perkebunan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; } = null!;
    }
}
