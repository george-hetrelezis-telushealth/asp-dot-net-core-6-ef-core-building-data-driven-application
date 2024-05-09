using Microsoft.EntityFrameworkCore;

namespace BethanysPieShopAdmin.Models
{
    public class BethanysPieShopDbContext : DbContext
    {
        public BethanysPieShopDbContext(DbContextOptions<BethanysPieShopDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Pie> Pies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BethanysPieShopDbContext).Assembly);

            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Pie>().ToTable("Pies");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderLines");

            //configuration using Fluent API
            modelBuilder.Entity<Category>()
            .Property(b => b.Name)
            .IsRequired();
        }
    }
}
