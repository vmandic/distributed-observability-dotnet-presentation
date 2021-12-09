using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace Ex2_App1
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostEnv;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnv)
        {
            Configuration = configuration;
            _hostEnv = hostEnv;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            services.AddControllers();

            services.AddHttpClient("Ex2_App2_Client")
                .ConfigureHttpClient(x => x.BaseAddress = new Uri("http://localhost:50002"));

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddAspNetCoreInstrumentation();
                builder.AddHttpClientInstrumentation();

                builder
                    .SetResourceBuilder(ResourceBuilder
                        .CreateDefault()
                        .AddService(_hostEnv.ApplicationName));

                if (_hostEnv.IsDevelopment())
                {
                    builder.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Debug);
                }

                // ref: https://www.jaegertracing.io/docs/1.29/getting-started/
                builder.AddJaegerExporter();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/get-data", async ctx =>
                {
                    ctx.Response.Headers.Add("Request-Id", Activity.Current?.TraceId.ToString() ?? string.Empty);

                    using var client = ctx.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("Ex2_App2_Client");

                    var resp = await client.GetStringAsync("/receive-data");

                    await ctx.Response.WriteAsync("Hi from App1, request sent to App2...\n");
                    await ctx.Response.WriteAsync($"App2 response is: { resp }");
                });
            });
        }
    }
}

//docker run -d --name jaeger \
//  -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 \
//  -p 5775:5775 / udp \
//  -p 6831:6831 / udp \
//  -p 6832:6832 / udp \
//  -p 5778:5778 \
//  -p 16686:16686 \
//  -p 14268:14268 \
//  -p 14250:14250 \
//  -p 9411:9411 \
//  jaegertracing / all -in-one:1.29

// docker run -d --name jaeger -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 -p 5775:5775/udp -p 6831:6831/udp -p 6832:6832/udp -p 5778:5778 -p 16686:16686 -p 14268:14268 -p 14250:14250 -p 9411:9411 jaegertracing/all-in-one:1.29