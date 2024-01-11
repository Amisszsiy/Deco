using Deco.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Deco.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Because Identity is installed
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Picture", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Carpet", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Cabinet", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Stary Night", Description = "Drawn by Van Goh", Price = 55000.00, SetPrice = 45000.00, CategoryId = 1, ImageUrl = ""},
                new Product { Id = 2, Name = "Scream", Description = "Famous screaming person", Price = 65000.00, SetPrice = 55000.00, CategoryId = 1, ImageUrl = "" },
                new Product { Id = 3, Name = "Fox fur 8*5", Description = "Fox fur carpet 8*5M", Price = 3500.00, SetPrice = 3000.00, CategoryId = 2, ImageUrl = "" },
                new Product { Id = 4, Name = "Bear fur 9*10", Description = "Bear fur carpet 9*10M", Price = 5000.00, SetPrice = 4000.00, CategoryId = 2, ImageUrl = "" },
                new Product { Id = 5, Name = "Tall White Cab170", Description = "White Cabinet 170*40*70", Price = 6000.00, SetPrice = 5000.00, CategoryId = 3, ImageUrl = "" },
                new Product { Id = 6, Name = "Short Dark Cab45", Description = "Dark Cabinet 45*40*70", Price = 2000.00, SetPrice = 1000.00, CategoryId = 3, ImageUrl = "" });
        }
    }
}