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
using AutoMapper;
using Business.Mapping;  // Add this namespace

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
            // Add necessary services for DB, Blazor, etc.
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<IVirtualAppointmentRepo, VirtualAppointmentRepo>();

            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));  // Ensure your mapping profile is properly registered

            services.AddDbContext<ApplicationDBContext>(option =>
                option.UseSqlServer(_config.GetConnectionString("DefaultConnection"))
            );

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMudServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
