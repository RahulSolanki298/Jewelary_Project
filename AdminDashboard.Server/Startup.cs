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
using AdminDashboard.Server.Service;
using AdminDashboard.Server.Service.IService;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;

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
            // ---------- Database ----------
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"))
            );

            // ---------- Server Limits ----------
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = long.MaxValue;
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = long.MaxValue;
                options.AutomaticAuthentication = false;
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 5368709120; // 5 GB
            });

            // ---------- Identity ----------
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            // ---------- Services ----------
            services.AddHttpClient();
            services.AddScoped<JwtTokenService>();
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<IVirtualAppointmentRepo, VirtualAppointmentRepo>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<ICategoryRepositry, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IB2BOrdersRepository, B2BOrdersRepository>();
            services.AddScoped<IB2COrdersRepository, B2COrdersRepository>();
            services.AddScoped<IDiamondRepository, DiamondRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddScoped<ILocalStorageService, LocalStorageService>();
            services.AddScoped<IDiamondPropertyRepository, DiamondPropertyRepository>();

            // ---------- External Libraries ----------
            services.AddBlazoredLocalStorage();
            services.AddMudServices();
            services.AddAutoMapper(typeof(MappingProfile));

            // ---------- Blazor + Razor ----------
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddServerSideBlazor();
            services.AddAuthorizationCore();
            services.AddHttpContextAccessor();

            // ---------- HttpClient ----------
            //services.AddHttpClient("MyApiClient", client =>
            //{
            //    client.BaseAddress = new Uri(SD.BaseApiUrl);
            //    client.Timeout = TimeSpan.FromMinutes(30);
            //});

            //services.AddScoped<HttpClient>(sp => new HttpClient
            //{
            //    BaseAddress = new Uri(SD.BaseApiUrl)
            //});

            // ---------- Excel ----------
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            // ---------- Exception Handling ----------
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain";

                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (exceptionHandlerFeature?.Error != null)
                    {
                        Log.Error(exceptionHandlerFeature.Error, "Unhandled Exception occurred.");
                        await context.Response.WriteAsync("An unexpected error occurred.");
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

            // ---------- Routing & Middleware ----------
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // ---------- Max Request Body Size Per Request ----------
            app.Use(async (context, next) =>
            {
                var maxRequestFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
                if (maxRequestFeature != null && maxRequestFeature.IsReadOnly == false)
                {
                    maxRequestFeature.MaxRequestBodySize = 5368709120; // 5 GB
                }
                await next();
            });

            // ---------- Seeding ----------
            SeedDatabase(app.ApplicationServices, dbInitializer).GetAwaiter().GetResult();

            // ---------- Endpoints ----------
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapRazorPages();
            });
        }

        private async Task SeedDatabase(IServiceProvider serviceProvider, IDbInitializer dbInitializer)
        {
            using var scope = serviceProvider.CreateScope();
            var scopedDbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await scopedDbInitializer.Initalize();
        }
    }

}
