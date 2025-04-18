using NguyenThanhTin_2122110125.Model;
using Microsoft.EntityFrameworkCore;

namespace NguyenThanhTin_2122110125.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 🔥 Thêm constructor không tham số
        public AppDbContext() { }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-KTJAEDP\\SQLEXPRESS;Database=Asp;User ID=sa;Password=sa;TrustServerCertificate=True;");
            }
        }
    }
}
