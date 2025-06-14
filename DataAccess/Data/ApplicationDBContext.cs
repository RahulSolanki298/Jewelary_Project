using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<LogEntry> LogEntries { get; set; }

        public DbSet<Diamond> Diamonds { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<VirtualAppointment> VirtualAppointment { get; set; }

        public DbSet<AcceptedVirtualAppointmentData> AcceptedVirtualAppointmentData { get; set; }

        public DbSet<CollectionHistory> CollectionHistory { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<CustomerOrders> CustomerOrders { get; set; }

        public DbSet<CustomerOrderItems> CustomerOrderItems { get; set; }

        public DbSet<CustomerOrderStatus> CustomerOrderStatus { get; set; }

        public DbSet<SubCategory> SubCategory { get; set; }

        public DbSet<ProductCollections> ProductCollections { get; set; }

        public DbSet<ProductProperty> ProductProperty { get; set; }

        public DbSet<ProductCaratSize> ProductCaratSize { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<ProductImages> ProductImages { get; set; }

        public DbSet<CoupanMaster> CoupanMaster { get; set; }

        public DbSet<BusinessAccount> BusinessAccount { get; set; }

        public DbSet<Orders> Orders { get; set; }

        public DbSet<OrderItems> OrderItems { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<BuyerOrderStatus> BuyerOrderStatus { get; set; }

        public DbSet<FileManager> FileManager { get; set; }

        public DbSet<ProductStyles> ProductStyles { get; set; }

        public DbSet<DiamondProperty> DiamondProperties { get; set; }

        public DbSet<DiamondShapeData> DiamondShapeData { get; set; }

        public DbSet<DiamondColorData> DiamondColorData { get; set; }

        public DbSet<ProductPrices> ProductPrices { get; set; }

        public DbSet<ProductWeight> ProductWeights { get; set; }

        public DbSet<EventSites> EventSites { get; set; }

        public DbSet<UserAddress> UserAddress { get; set; }

        public DbSet<CompanyData> CompanyData { get; set; }

        public DbSet<DiamondFileUploadHistory> DiamondFileUploadHistory { get; set; }

        public DbSet<ProductCollectionItems> ProductCollectionItems { get; set; }

        public DbSet<ProductFileUploadHistory> ProductFileUploadHistory { get; set; }

        public DbSet<DiamondHistory> DiamondHistory { get; set; }

        public DbSet<ProductHistory> ProductHistory { get; set; }

        public DbSet<ProductStyleItems> ProductStyleItems { get; set; }

        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<HomePageSetting> HomePageSetting { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }

        public DbSet<DiamondData> DiamondData { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiamondData>().HasNoKey(); // or .ToView("YourView") if needed
            modelBuilder.Entity<DiamondShapeData>().ToTable("DiamondShapeData").HasNoKey();
            modelBuilder.Entity<DiamondColorData>().ToTable("DiamondColorData").HasNoKey();

            // DiamondHistory
            modelBuilder.Entity<DiamondHistory>(e =>
            {
                e.Property(d => d.UnitPrice).HasPrecision(18, 2);
                e.Property(d => d.Width).HasPrecision(10, 3);
                e.Property(d => d.Amount).HasPrecision(18, 2);
                e.Property(d => d.CAngle).HasPrecision(8, 3);
                e.Property(d => d.CHt).HasPrecision(8, 3);
                e.Property(d => d.Carat).HasPrecision(8, 3);
                e.Property(d => d.Depth).HasPrecision(8, 3);
                e.Property(d => d.Dia).HasPrecision(8, 3);
                e.Property(d => d.Discount).HasPrecision(10, 2);
                e.Property(d => d.GirdleOpen).HasPrecision(8, 3);
                e.Property(d => d.Height).HasPrecision(8, 3);
                e.Property(d => d.Length).HasPrecision(8, 3);
                e.Property(d => d.PAngle).HasPrecision(8, 3);
                e.Property(d => d.PHt).HasPrecision(8, 3);
                e.Property(d => d.Price).HasPrecision(18, 2);
                e.Property(d => d.Quantity).HasPrecision(10, 2);
            });

            // OrderItems
            modelBuilder.Entity<OrderItems>(e =>
            {
                e.Property(o => o.Dicount).HasPrecision(5, 2);
                e.Property(o => o.DicountAmount).HasPrecision(10, 2);
            });

            // CoupanMaster
            modelBuilder.Entity<CoupanMaster>()
                .Property(c => c.Discount)
                .HasPrecision(5, 2);

            // CustomerOrderItems
            modelBuilder.Entity<CustomerOrderItems>(e =>
            {
                e.Property(c => c.Prices).HasPrecision(18, 2);
                e.Property(c => c.TotalAmount).HasPrecision(18, 2);
                e.Property(c => c.UnitPrice).HasPrecision(18, 2);
            });

            // CustomerOrders
            modelBuilder.Entity<CustomerOrders>(e =>
            {
                e.Property(c => c.Dicount).HasPrecision(5, 2);
                e.Property(c => c.DicountAmount).HasPrecision(10, 2);
                e.Property(c => c.NetAmount).HasPrecision(18, 2);
            });

            // Diamond
            modelBuilder.Entity<Diamond>(e =>
            {
                e.Property(d => d.Amount).HasPrecision(18, 2);
                e.Property(d => d.CAngle).HasPrecision(8, 3);
                e.Property(d => d.CHt).HasPrecision(8, 3);
                e.Property(d => d.Carat).HasPrecision(8, 3);
                e.Property(d => d.Depth).HasPrecision(8, 3);
                e.Property(d => d.Dia).HasPrecision(8, 3);
                e.Property(d => d.Discount).HasPrecision(10, 2);
                e.Property(d => d.GirdleOpen).HasPrecision(8, 3);
                e.Property(d => d.Height).HasPrecision(8, 3);
                e.Property(d => d.Length).HasPrecision(8, 3);
                e.Property(d => d.PAngle).HasPrecision(8, 3);
                e.Property(d => d.PHt).HasPrecision(8, 3);
                e.Property(d => d.Price).HasPrecision(18, 2);
                e.Property(d => d.Quantity).HasPrecision(10, 2);
                e.Property(d => d.RAP).HasPrecision(10, 2);
                e.Property(d => d.RapAmount).HasPrecision(18, 2);
                e.Property(d => d.RatePct).HasPrecision(5, 2);
                e.Property(d => d.Ratio).HasPrecision(8, 3);
                e.Property(d => d.Table).HasPrecision(8, 3);
                e.Property(d => d.UnitPrice).HasPrecision(18, 2);
                e.Property(d => d.Width).HasPrecision(8, 3);
            });

            // ProductProperty.Name should not allow nulls
            modelBuilder.Entity<ProductProperty>()
                .Property(p => p.Name)
                .IsRequired();

            

        }

    }
}
