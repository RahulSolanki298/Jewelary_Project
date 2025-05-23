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
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10737418240; // 10 GB
            });

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
                    options.LoginPath = "~/Account/Index";            // Adjust if route differs
                    options.AccessDeniedPath = "~/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                });

            ConfigureFormOptions(services);

            // Authorization support
            services.AddAuthorization();

            // MVC & Razor
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });
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

            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            InitializeDatabase(dbInitializer).GetAwaiter().GetResult();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
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
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }
    }
}
