using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Formatting.Compact;
using System;

namespace Ex1_HelloWorldLogging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // not so super cool as you create config... (use https://github.com/serilog/serilog-aspnetcore#two-stage-initialization) with CreateBootstrapLogger() instead
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // the global non-microsoft logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                //.WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

            try
            {
                //Log.Information("App is starting...");
                var appHost = CreateHostBuilder(args).Build();
                appHost.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "You app did not manage to start");
                throw;
            }
            finally
            {
                // make sure the log finishes before the app shuts down
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(opts => {

                    // remove all default providers... recommended
                    //opts.ClearProviders();
                    //opts.AddConsole();
                    //opts.AddEventLog(evOpts =>
                    //{
                    //    evOpts.LogName = "Ex1_HelloWorldLogging";
                    //    evOpts.SourceName = "Ex1_HelloWorldLogging";
                    //});
                })
                // will use the Log.Logger instance we set up in Main()
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

// start seq with docker locally
// docker run --rm --name seq -d -e ACCEPT_EULA=Y -p 5341:5341 -p 8888:80 datalust/seq:latest

// https://benfoster.io/blog/serilog-best-practices
