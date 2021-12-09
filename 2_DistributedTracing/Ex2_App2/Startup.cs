using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Ex2_App2
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
            services.AddControllers();
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

                endpoints.MapGet("/receive-data", async ctx =>
                {
                    var traceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
                    var spanId = Activity.Current?.SpanId.ToString() ?? string.Empty;
                    var parentId = Activity.Current?.ParentId ?? string.Empty;

                    ctx.Response.Headers.Add("Request-Id", Activity.Current?.TraceId.ToString() ?? string.Empty);

                    var logger = ctx.RequestServices.GetRequiredService<ILogger<Startup>>();

                    logger.LogWarning("Someone pinged /receive-data endpoint!\nTrace ID: {traceId}\nSpan ID: {spanId}\nParent ID: {parentId}", traceId, spanId, parentId);

                    await ctx.Response.WriteAsync($"Hi this is from App2, here is a timestamp: {DateTime.Now:s}");
                });
            });
        }
    }
}
