using Backend_Test.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Test.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Images> images { get; set; }
    }
}
