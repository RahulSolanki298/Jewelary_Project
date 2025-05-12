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

namespace B2C_ECommerce
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.CommandTimeout(300)) // Timeout in seconds (5 minutes)
            );

            // Add Identity services
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();
            ConfigureRepositories(services);

            services.AddHttpClient();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddHttpClient("API", client =>
            {
                client.BaseAddress = new Uri(SD.BaseApiUrl); 
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IDiamondService, DiamondService>();
            services.AddScoped<IProductService, ProductService>();
            //services.AddScoped<IB2COrdersRepository, B2COrdersRepository>();

            services.AddControllers();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            //services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            //services.AddScoped<IVirtualAppointmentRepo, VirtualAppointmentRepo>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICategoryRepositry, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IDiamondPropertyRepository, DiamondPropertyRepository>();
            services.AddScoped<IDiamondRepository, DiamondRepository>();
            services.AddScoped<IB2COrdersRepository, B2COrdersRepository>();
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
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 

                // Configure route for MVC controllers
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"); // Default MVC route
            });
        }
    }
}
