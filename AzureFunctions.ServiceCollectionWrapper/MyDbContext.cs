using Microsoft.EntityFrameworkCore;

namespace AzureFunctions.ServiceCollectionWrapper
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}