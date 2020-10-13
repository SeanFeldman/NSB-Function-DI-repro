using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AzureFunctions.ASBTrigger.DI
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable($"{nameof(User)}s");
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }

    class DbInitializer
    {
        public static async Task Seed(MyDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Users.AnyAsync())
            {
                return;
            }

            var user = new User {UserId = 1, Email = "feldman.sean@gmail.com", IsActive = true};

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
    }
}