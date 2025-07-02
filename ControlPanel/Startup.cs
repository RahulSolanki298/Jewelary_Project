using Business.Repository;
using Business.Repository.IRepository;
using ControlPanel.Services;
using ControlPanel.Services.IServices;
using DataAccess.Data;
using DataAccess.Entities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ControlPanel
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Database & Identity

            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();

            #endregion

            #region Cookie Authentication

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(2); // Long-lived cookie
                options.SlidingExpiration = true;

                options.Cookie.Name = "ControlPanelAuth";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                                  .AddCookie(options =>
                                  {
                                      options.LoginPath = "/Account/Login";
                                      options.ExpireTimeSpan = TimeSpan.FromHours(2);     // Set longer expiration
                                      options.SlidingExpiration = true;                   // Refresh cookie on activity
                                      options.Events.OnRedirectToLogin = context =>
                                      {
                                          //if (context.Request.IsAjaxRequest())
                                          //{
                                          //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                          //    return Task.CompletedTask;
                                          //}
                                          context.Response.Redirect(context.RedirectUri);
                                          return Task.CompletedTask;
                                      };
                                  });



            #endregion

            #region Session & HttpContext

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddHttpContextAccessor();

            #endregion

            #region Razor + MVC + FluentValidation

            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation()
                    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddControllersWithViews()
                    .AddViewOptions(opt =>
                    {
                        opt.HtmlHelperOptions.ClientValidationEnabled = true;
                    });

            #endregion

            #region Custom Services & Repositories

            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ILogEntryRepository, LogEntryRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICategoryRepositry, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IB2BOrdersRepository, B2BOrdersRepository>();
            services.AddScoped<IB2COrdersRepository, B2COrdersRepository>();
            services.AddScoped<IDiamondRepository, DiamondRepository>();
            services.AddScoped<IDiamondPropertyRepository, DiamondPropertyRepository>();
            services.AddScoped<IProductStyleRepository, ProductStyleRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<ICollectionRepository, CollectionRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();

            #endregion

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

            services.AddAuthorization();
        }

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

            // Basic security headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();               // Use session before auth
            app.UseAuthentication();        // Login cookies middleware
            app.UseAuthorization();         // Enforce policies

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
                File.WriteAllText("startup-pipeline-error.log", ex.ToString());
                throw;
            }
        }
    }
}
