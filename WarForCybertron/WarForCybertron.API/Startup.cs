using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WarForCybertron.Common.Configuration;
using WarForCybertron.Repository;

namespace WarForCybertron.API
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

            services.Configure<ConfigSettings>(Configuration);

            var configSettings = new ConfigSettings();
            Configuration.GetSection(nameof(ConfigSettings)).Bind(configSettings);

            var connectionString = $"Server={configSettings.SQL_HOST};Database={configSettings.SQL_DBNAME};User ID={configSettings.SQL_USER};Password={configSettings.SQL_PASSWORD};";

            services.AddEntityFrameworkSqlServer()
              .AddDbContextPool<WarForCybertronContext>((serviceProvider, optionsBuilder) =>
                  {
                      optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                      optionsBuilder.UseSqlServer(connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                      optionsBuilder.UseInternalServiceProvider(serviceProvider);
                  }
              );

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddConsole();
                builder.AddFilter("Microsoft", LogLevel.Warning);
            });

            services.AddScoped(typeof(WarForCybertronRepository<,>), typeof(WarForCybertronRepository<,>));
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
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<WarForCybertronContext>();
                context.Database.Migrate();
            }
        }
    }
}
