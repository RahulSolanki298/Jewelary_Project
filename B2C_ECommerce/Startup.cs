using System;
using B2C_ECommerce.IServices;
using B2C_ECommerce.Services;
using Business.Repository;
using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace B2C_ECommerce
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            Log.Logger = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                 .CreateLogger();
            Log.Information("Starting up the service...");


            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.CommandTimeout(300))
            );

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();

            ConfigureRepositories(services);

            services.AddHttpClient();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddHttpClient("API", client =>
            {
                client.BaseAddress = new Uri(SD.AdminPath);
                //client.BaseAddress = new Uri(SD.BaseApiUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IDiamondService, DiamondService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<IProductStyleRepository, ProductStyleRepository>();
            
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICategoryRepositry, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IDiamondPropertyRepository, DiamondPropertyRepository>();
            services.AddScoped<IDiamondRepository, DiamondRepository>();
            services.AddScoped<IB2COrdersRepository, B2COrdersRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseCors("AllowAll");
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

}
