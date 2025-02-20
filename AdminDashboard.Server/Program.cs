using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace AdminDashboard.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog at the start of the application.
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug() // Log level can be adjusted (Debug, Information, etc.)
                        .WriteTo.File($"Logs/log-{DateTime.Now.ToString("ddMMyyyy")}.txt") // Rolling daily logs with ddMMyyyy date format
                        .CreateLogger();


            try
            {
                // Create and run the host
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush(); // Ensure logs are flushed before app exits
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog(); // Use Serilog for logging
    }
}
