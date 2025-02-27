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
using Business.Mapping;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using AdminDashboard.Service.IService;
using AdminDashboard.Service;
using System;

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

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(MappingProfile));  // Ensure your mapping profile is properly registered

            // Add necessary services for DB, Blazor, etc.
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<IVirtualAppointmentRepo, VirtualAppointmentRepo>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<ICategoryRepositry, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            
            services.AddRazorPages();
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
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
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
