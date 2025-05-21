using Business.Repository;
using Business.Repository.IRepository;
using ControlPanel.Services;
using ControlPanel.Services.IServices;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Models.ExternalConnectors;
using System;
using System.Threading.Tasks;

namespace ControlPanel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Database context
            services.AddDbContext<ApplicationDBContext>(options =>
            options.UseSqlServer(
        Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    ));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            // Dependency injection for repositories
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<ICategoryRepositry, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IB2BOrdersRepository, B2BOrdersRepository>();
            services.AddScoped<IB2COrdersRepository, B2COrdersRepository>();
            services.AddScoped<IDiamondRepository, DiamondRepository>();
            services.AddScoped<IDiamondPropertyRepository, DiamondPropertyRepository>();
            //services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();
            // Cookie Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Admin/Login";            // Adjust if route differs
                    options.AccessDeniedPath = "/Admin/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                });

            ConfigureFormOptions(services);

            // Authorization support
            services.AddAuthorization();

            // MVC & Razor
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        private void ConfigureFormOptions(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = 5368709120; // 5 GB
                options.MemoryBufferThreshold = int.MaxValue;
            });
        }

        // Configure HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication(); // Must come before UseAuthorization
            app.UseAuthorization();

            // Initialize database with seed data
            InitializeDatabase(dbInitializer).GetAwaiter().GetResult();

            app.UseEndpoints(endpoints =>
            {
                // Area routing (must come before default route)
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                // Default routing
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Index}/{id?}");
            });

        }

        private async Task InitializeDatabase(IDbInitializer dbInitializer)
        {
            try
            {
                await dbInitializer.Initalize();
            }
            catch (Exception ex)
            {
                // Optional: log error or throw exception
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }
    }
}
