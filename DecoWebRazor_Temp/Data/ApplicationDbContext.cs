using DecoWebRazor_Temp.Model;
using Microsoft.EntityFrameworkCore;

namespace DecoWebRazor_Temp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Picture", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Carpet", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Cabinet", DisplayOrder = 3 }
                );
        }
    }
}
