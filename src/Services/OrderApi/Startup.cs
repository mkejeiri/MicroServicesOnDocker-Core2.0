using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Threading;
using MicroServicesOnDocker.Services.OrderApi.Data;
using MicroServicesOnDocker.Services.OrderApi.Infrastructure.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
namespace MicroServicesOnDocker.Services.OrderApi
{
    public class Startup
    {

        // private readonly string _connectionString;
        ILogger _logger;

        private string _connectionString;
        //private string _connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<Startup>();
            Configuration = configuration;
            //_connectionString = $@"Server={Environment.GetEnvironmentVariable("MYSQL_SERVICE_NAME") ?? "mysqlserver"};
            //                       Port={ "3406" };
            //                       Database={Environment.GetEnvironmentVariable("MYSQL_DATABASE")};
            //                       Uid={Environment.GetEnvironmentVariable("MYSQL_USER")};
            //                       Pwd={Environment.GetEnvironmentVariable("MYSQL_PASSWORD")}";

            //_connectionString = Configuration["ConnectionString"];
            //_connectionString = "server=mysqlserver;port=3406;database=OrdersDb;Uid=lokum1;pwd=my-secret-pw";
        }



        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(
                    options => options.Filters.Add(typeof(HttpGlobalExceptionFilter))
                )
                .AddJsonFormatters(
                    opt =>
                    {
                        opt.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        opt.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    }
                )
                .AddApiExplorer();

            //services.AddEntityFrameworkMySql()
            //    .AddDbContext<OrdersContext>(options =>
            //        {
            //            options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
            //                mySqlOptionsAction: sqlOptions =>
            //                {
            //                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

            //                });
            //        },
            //        ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            //);

            services.Configure<OrderSettings>(Configuration);

            //WaitForDBInit(_connectionString);

            //var hostname = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "mssqlserver";
            //var password = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? "MyProduct!123";
            //var database = Environment.GetEnvironmentVariable("DATABASE") ?? "OrdersDb";
            //var connectionString = $"Server={hostname};Database={database};User ID=sa;Password={password};";
            //var server = Configuration["DatabaseServer"];
            //var database = Configuration["DatabaseName"];
            //var user = Configuration["DatabaseUser"];
            //var password = Configuration["DatabaseUserPassword"];
            //var connectionString = $"Server={server}; Database={database};User={user};Password={password}";

            //var hostname = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "localhost,1490";
            //var password = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? @"!5F&svw1234";
            //var database = Environment.GetEnvironmentVariable("DATABASE") ?? "OrdersDb";

            //var connectionString = $"Server={hostname};Database={database};User ID=sa;Password={password};";

            var connectionString = Configuration["ConnectionString"];
            services.AddDbContext<OrdersContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });

                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                //Check Client vs. Server evaluation: https://docs.microsoft.com/en-us/ef/core/querying/client-eval
            });

            ConfigureAuthService(services);

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Shopping Order HTTP API",
                    Version = "v1",
                    Description = "The Shopping Order Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });
                //this added so swagger could be recognized as allowed client to talk to TokenServiceApi
                //behind the scenes, it reaches "/connect/authorize" and "/connect/token" endpoints
                //with one of allowed scopes as "Order" (i.e dictionary element below) to get a valid token
                //so it could talk to OrderApi (OrderController with Authorized)
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/authorize",
                    TokenUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "order", "Order Api" }
                    }
                });

                //adding a Check into the pipeline to observe if any method/controller has an [Authorized] attributes on it
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }

       

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                //No secured socket layer is used here
                options.RequireHttpsMetadata = false;
                //cart is the name of the ApiResource("cart", "Shopping Cart Api") as registered in TokenServiceApi (config.cs),
                //it should be an exact match!
                options.Audience = "order";
            });
        }

        //private void WaitForDBInit(string connectionString)
        //{
        //    var connection = new MySqlConnection(connectionString);
        //    int retries = 1;
        //    while (retries < 7)
        //    {
        //        try
        //        {
        //            Console.WriteLine("Connecting to db. Trial: {0}", retries);
        //            connection.Open();
        //            connection.Close();
        //            break;
        //        }
        //        catch (MySqlException)
        //        {
        //            Thread.Sleep((int)Math.Pow(2, retries) * 1000);
        //            retries++;
        //        }
        //    }
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, OrdersContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMvc();
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }


            //app.UseStaticFiles();
            //this ensure migration are applied
            context.Database.Migrate();
            app.UseCors("CorsPolicy");
            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Order.API V1");
                    //Old
                    //c.ConfigureOAuth2("orderswaggerui", "", "", "Order Swagger UI");

                    c.OAuthClientId("orderswaggerui");
                    c.OAuthClientSecret(null);
                    c.OAuthRealm(null);
                    c.OAuthAppName("order Swagger UI");
                    c.OAuthScopeSeparator(null);
                    c.OAuthAdditionalQueryStringParams(null);
                    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });
        }
    }
}
