using DomainModels.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class HotelContext : DbContext
    {
        public HotelContext(DbContextOptions<HotelContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Extra> Extras { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<Admin> Admins { get; set; }

       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            

            // Seed the roles to the database
            List<IdentityRole> roles = new List<IdentityRole>()
            {   
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" }
            };

            var hasher = new PasswordHasher<User>();

            User user = new User
            {
                UserName = "admin",
                Email = "admin@admin.com",
                FirstName = "Admin",
                LastName = "Admin",
                PhoneNumber = "123456789",
                PasswordHash = hasher.HashPassword(null, "admin")
            };

            builder.Entity<User>().HasData(user);
            builder.Entity<IdentityRole>().HasData(roles);
            
        }
    }
}
