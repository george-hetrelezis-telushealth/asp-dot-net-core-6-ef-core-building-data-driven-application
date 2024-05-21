using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Models
{
    public class BethanysPieShopDbContext: DbContext
    {
        public BethanysPieShopDbContext(DbContextOptions<BethanysPieShopDbContext> options): base(options)
        {}

        public DbSet<Category> Categories { get; set; }
        public DbSet<Pie> Pies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Pie>().ToTable("Pies"); 
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderLines");

            modelBuilder.Entity<Category>()
                .Property(b => b.Name)
                .IsRequired();
        }
    }
}
