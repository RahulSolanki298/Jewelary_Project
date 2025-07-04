using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

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

        public DbSet<ProductMaster> ProductMaster { get; set; }
        public DbSet<ProductMasterHistory> ProductMasterHistory { get; set; }


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

        public async Task<ProductMstResponse> SaveNewProductListToDbAsync(
       List<ProductDTO> products,
       string categoryName,
       string userId,
       int fileHistoryId)
        {
            var response = new ProductMstResponse();

            try
            {
                var table = CreateProductDataTable(products); // Your existing method

                var conn = (SqlConnection)Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                using var command = new SqlCommand("SaveNewProductList", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var tvpParam = command.Parameters.AddWithValue("@Products", table);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "ProductDTOType";

                command.Parameters.AddWithValue("@CategoryName", categoryName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserId", userId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FileHistoryId", fileHistoryId);

                await command.ExecuteNonQueryAsync();

                    response.Status = true;
                response.Message = "Product list successfully saved.";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }

        private DataTable CreateProductDataTable(List<ProductDTO> products)
        {
            var table = new DataTable();
            table.Columns.Add("Sku", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("ColorName", typeof(string));
            table.Columns.Add("Karat", typeof(string));
            table.Columns.Add("CenterCaratName", typeof(string));
            table.Columns.Add("CenterShapeName", typeof(string));
            table.Columns.Add("AccentStoneShapeName", typeof(string));
            table.Columns.Add("WholesaleCost", typeof(decimal));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("Length", typeof(string));
            table.Columns.Add("BandWidth", typeof(string));
            table.Columns.Add("ProductType", typeof(string));
            table.Columns.Add("GoldWeight", typeof(string));
            table.Columns.Add("Grades", typeof(string));
            table.Columns.Add("MMSize", typeof(string));
            table.Columns.Add("NoOfStones", typeof(int));
            table.Columns.Add("DiaWT", typeof(decimal));
            table.Columns.Add("Certificate", typeof(string));
            table.Columns.Add("IsReadyforShip", typeof(bool));
            table.Columns.Add("CTW", typeof(decimal));
            table.Columns.Add("VenderName", typeof(string));
            table.Columns.Add("VenderStyle", typeof(string));
            table.Columns.Add("Diameter", typeof(string));
            table.Columns.Add("FileHistoryId", typeof(int));
            table.Columns.Add("StyleName", typeof(string));
            table.Columns.Add("CollectionName", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Type", typeof(string));

            foreach (var p in products)
            {
                table.Rows.Add(
                    p.Sku, p.Title, p.ColorName, p.Karat, p.CenterCaratName, p.CenterShapeName, p.AccentStoneShapeName,
                    p.WholesaleCost ?? 0, p.Price ?? 0, p.UnitPrice ?? 0, p.Length, p.BandWidth, p.ProductType,
                    p.GoldWeight, p.Grades, p.MMSize, p.NoOfStones ?? 0, p.DiaWT ?? 0, p.Certificate,
                    p.IsReadyforShip ?? false, Convert.ToDecimal(p.CTW ?? "0"), p.VenderName, p.VenderStyle, p.Diameter,
                    p.FileHistoryId ?? 0, p.StyleName, p.CollectionName, p.Description, p.Type
                );
            }

            return table;
        }
    }
}
