using Microsoft.AspNetCore.Builder;
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
            // Configure Serilog to capture only errors and above (including fatal exceptions)
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Error() // Logs only Error and Fatal logs
                        .WriteTo.File($"Logs/errors-{DateTime.Now:ddMMyyyy}.log", rollingInterval: RollingInterval.Day)
                        .CreateLogger();

            try
            {
                // Run the application
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush(); // Ensure logs are flushed before application exits
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
