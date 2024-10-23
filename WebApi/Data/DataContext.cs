using Microsoft.EntityFrameworkCore;
using WebApi.Models;


namespace WebApi.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {}
        public DbSet<User> Users { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Seeding Admin User
            modelBuilder.Entity<User>().HasData(new User { 
                UserId = -1,
                Username = "superadmin", 
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "admin@gmail.com",
                Role = UserRole.Admin,
                Department = "Admin"
            });
        }
    }

}
