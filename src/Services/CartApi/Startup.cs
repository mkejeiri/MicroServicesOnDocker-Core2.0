using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MicroServicesOnDocker.Services.CartApi.Infrastructure.Filters;
using MicroServicesOnDocker.Services.CartApi.Model;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;

namespace MicroServicesOnDocker.Services.CartApi
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
            //add the custom HttpGlobalExceptionFilter to handle exceptions globally
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();

            services.Configure<CartSettings>(Configuration);
            ConfigureAuthService(services);

            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CartSettings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);

                configuration.ResolveDns = true;
                //allows a retry after a connection failure
                configuration.AbortOnConnectFail = false;

                return ConnectionMultiplexer.Connect(configuration);
            });
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Shopping Cart HTTP API",
                    Version = "v1",
                    Description = "The Shopping Cart Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });
                //this added so swagger could be recognized as allowed client to talk to TokenServiceApi
                //behind the scenes, it reaches "/connect/authorize" and "/connect/token" endpoints
                //with one of allowed scopes as "cart" (i.e dictionary element below) to get a valid token
                //so it could talk to CartApi (CartController with Authorized)
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/authorize",
                    TokenUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "cart", "Cart Api" }
                    }
                });
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICartRepository, RedisCartRepository>();
         //   services.AddTransient<IIdentityService, IdentityService>();
        }

        //Asp.net core has already a  JwtHandler middleware to handle JWT token
        //hence all comes down into the following configuration
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
                options.RequireHttpsMetadata = false;
                //No secured socket layer is used here
                options.RequireHttpsMetadata = false;
                //basket is the name of the ApiResource("cart", "Shopping Cart Api") as registered in TokenServiceApi (config.cs),
                //it should be an exact match!
                options.Audience = "cart";
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "cart.API V1");
                   //Old
                   //c.ConfigureOAuth2("cartswaggerui", "", "", "Cart Swagger UI");

                   c.OAuthClientId("cartswaggerui");
                   c.OAuthClientSecret("");
                   c.OAuthRealm("");
                   c.OAuthAppName("Cart Swagger UI");
                   //c.OAuthScopeSeparator(" ");
                   //c.OAuthAdditionalQueryStringParams(new { foo = "bar" });
                   //c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
               });
        }
    }
}
