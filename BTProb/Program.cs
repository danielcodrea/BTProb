using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BTProb.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using BTProb.Models.Settings;

namespace BTProb
{
    public class Program
    {
        
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
           //there is a known error of Directory.GetCurrentDirectory, not setting the right path sometimes, so I needed to create a static workaround
           .SetBasePath(AppContext.BaseDirectory.Replace("\\bin\\Debug\\netcoreapp2.2\\", string.Empty))
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();
        public static void Main(string[] args)
        {
            try
            {
                //CurrentDirectoryHelpers.SetCurrentDirectory();
                var serilogSettings = Configuration.GetSection(nameof(SerilogSettings)).Get<SerilogSettings>();
                Log.Logger = new LoggerConfiguration()
                .WriteTo
                .File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\netcoreapp2.2\\",string.Empty), serilogSettings.WriteTo[0].Args.path),
                    rollingInterval: serilogSettings.WriteTo[0].Args.rollingInterval,
                    shared: serilogSettings.WriteTo[0].Args.shared,
                    outputTemplate: serilogSettings.WriteTo[0].Args.outputTemplate)
                //.ReadFrom.Configuration(Configuration) // this works only on windows, the above .WriteTo.File combination works on both windows and linux
                .CreateLogger();

            
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
