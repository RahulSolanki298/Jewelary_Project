using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AdminDashboard.Service.IService;
using AdminDashboard.Service;
using Business.Repository;
using Business.Repository.IRepository;
using Business.Mapping;
using MudBlazor.Services;
using DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

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
            // Configure Database
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"))
            );

            // Configure Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Register Repositories
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<IVirtualAppointmentRepo, VirtualAppointmentRepo>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<ICategoryRepositry, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IB2BOrdersRepository, B2BOrdersRepository>();

            services.AddHttpContextAccessor();
            services.AddServerSideBlazor();
            services.AddRazorPages();
            services.AddMudServices();

            // Configure HttpClient
            services.AddScoped<HttpClient>(sp => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:4050/") // Your API base address
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            // Add centralized exception logging
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                    if (exceptionHandlerPathFeature?.Error != null)
                    {
                        Log.Error(exceptionHandlerPathFeature.Error, "Unhandled Exception occurred.");
                    }
                });
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Database Seeding
            Seeding(app.ApplicationServices, dbInitializer);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapRazorPages();
            });
        }

        private async void Seeding(IServiceProvider serviceProvider, IDbInitializer dbInitializer)
        {
            using var scope = serviceProvider.CreateScope();
            var scopedDbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await scopedDbInitializer.Initalize();
        }
    }
}
