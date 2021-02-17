using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using WarForCybertron.Common.Configuration;
using WarForCybertron.Repository;
using WarForCybertron.Service.Implementations;
using WarForCybertron.Service.Interfaces;

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

            services.Configure<ConfigSettings>(Configuration.GetSection(nameof(ConfigSettings)));

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

            services.AddScoped(typeof(IWarForCybertronRepository<,>), typeof(WarForCybertronRepository<,>));

            services.AddScoped<IWarForCybertronService, WarForCybertronService>();

            services.AddAutoMapper(Assembly.Load("WarForCybertron.Service"));

#if DEBUG
            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "War for Cybertron API",
                    Description = "The API for War for Cybertron",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact { Name = "Adam Lamping", Email = "adamrlamping@gmail.com", Url = new Uri("http://twitter.com/adamlamping") },
                    License = new OpenApiLicense { Name = "Apache 2.0", Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0") }
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                c.IncludeXmlComments(Path.Combine(basePath + "/war-for-cybertron-api.xml"));
            });
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/ts-{Date}.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<WarForCybertronContext>();
            context.EnsureSeedData(Configuration.GetSection(nameof(ConfigSettings))["TRANSFORMERS_WITH_GOD_MODE"]);
            context.Database.Migrate();
        }
    }
}
