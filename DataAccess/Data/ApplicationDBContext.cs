using DataAccess.Entities;
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

        public DbSet<DiamondData> DiamondData { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiamondData>().ToTable("Diamond").HasNoKey();
            modelBuilder.Entity<DiamondShapeData>().ToTable("DiamondShapeData").HasNoKey();
            modelBuilder.Entity<DiamondColorData>().ToTable("DiamondColorData").HasNoKey();
        }
    }
}
