using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace ControlPanel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    try
                    {
                        webBuilder.UseStartup<Startup>();

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                });
    }
}
