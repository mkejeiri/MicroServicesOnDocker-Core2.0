﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MicroServicesOnDocker.Services.ProductCatalogApi.Data;
using MicroServicesOnDocker.Services.ProductCatalogApi.Domain.Repository;
using Swashbuckle.AspNetCore.Swagger;

namespace MicroServicesOnDocker.Services.ProductCatalogApi
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
            services.AddSwaggerGen(swaggerGenOptions =>
            {
                swaggerGenOptions.SwaggerDoc("v1", new Info
                {
                    Title = "ProductCatalogApi",
                    Version = "v1",
                    Description = "Microservice on docker for ProductCatalogApi"
                });
            });
            services.Configure<AppSettings>(Configuration);
            services.AddScoped<ICatalogRepository, CatalogSqlServerRepository>();

            //at runtime for dockerized build the connection string will be read from the docker-compose.yml
            var server = Configuration["DatabaseServer"];
            var database = Configuration["DatabaseName"];
            var user = Configuration["DatabaseUser"];
            var password = Configuration["DatabaseUserPassword"];
            var connectionString = $"Server={server}; Database={database};User={user};Password={password}";


            services.AddDbContext<CatalogDbContext>(options =>
#if DEBUG
                //connectionString is defined in the AppSetting json file
                options.UseSqlServer(Configuration["connectionString"]));
#else
                //connectionString is Docker Environment defined in docker-compose file
                options.UseSqlServer(connectionString));
#endif
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(swaggerUiOptions =>
            {
                swaggerUiOptions.SwaggerEndpoint($"/swagger/v1/swagger.json", $"ProductCatalogApi V1");
            });
            app.UseMvc();
        }
    }
}
