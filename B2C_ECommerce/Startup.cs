using System;
using B2C_ECommerce.IServices;
using B2C_ECommerce.Services;
using Business.Repository;
using Business.Repository.IRepository;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
