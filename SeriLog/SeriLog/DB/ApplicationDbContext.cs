using Microsoft.EntityFrameworkCore;
using SeriLog.Models;

namespace SeriLog.DB
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
    }
}
