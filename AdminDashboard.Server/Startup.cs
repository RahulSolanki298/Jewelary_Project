using Business.Repository;
using Business.Repository.IRepository;
using DataAccess.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using AdminDashboard.Service.IService;
using AdminDashboard.Service;
using System;
using System.Net.Http;
using Business.Mapping;

namespace AdminDashboard.Server
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register AutoMapper
            services.AddDbContext<ApplicationDBContext>(option =>
                option.UseSqlServer(_config.GetConnectionString("DefaultConnection"))
            );

        //    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        //.AddEntityFrameworkStores<ApplicationDBContext>();


            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(typeof(MappingProfile));  // Ensure your mapping profile is properly registered

            // Add necessary services for DB, Blazor, etc.
            services.AddScoped<HttpClient>(sp =>
            {
                var baseAddress = new Uri("https://localhost:4050/"); // Your API base address
                return new HttpClient { BaseAddress = baseAddress };
            });
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<IVirtualAppointmentRepo, VirtualAppointmentRepo>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<ICategoryRepositry, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddServerSideBlazor();
            services.AddMudServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            // Add logging middleware
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts(); // Security enhancement for production
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Seeding data in a scope to ensure proper disposal
            Seeding(app.ApplicationServices, dbInitializer);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                //endpoints.MapFallbackToPage("/Account/Login");
                endpoints.MapRazorPages();
            });
        }

        private async void Seeding(IServiceProvider serviceProvider, IDbInitializer dbInitializer)
        {
            // Create a scope to resolve scoped services
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedDbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

                // Ensure seeding is done asynchronously
                await scopedDbInitializer.Initalize();
            }
        }
    }
}
