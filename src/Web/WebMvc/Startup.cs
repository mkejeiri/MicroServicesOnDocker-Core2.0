using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MicroServicesOnDocker.Web.WebMvc.Infrastructure;
using MicroServicesOnDocker.Web.WebMvc.Models;
using MicroServicesOnDocker.Web.WebMvc.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MicroServicesOnDocker.Web.WebMvc
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
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.Configure<AppSettings>(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpClient, CustomHttpClient>(); 
            services.AddSingleton<HttpClient, HttpClient>();

            services.AddTransient<IIdentityService<ApplicationUser>, IdentityService>();
            services.AddTransient<ICartService, CartService>();
            services.AddTransient<IOrderService, OrderService>();

            //double check
            services.AddSingleton<ILogger<IHttpClient>, Logger<IHttpClient>>();
            services.AddTransient<ICatalogService, CatalogService>();
            //services.AddMvc();

            //We use cookies for authentications
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = Configuration.GetValue<string>("CallBackUrl");

            //We use OpenId packages which acts as a middleware for authentication and where to redirect the client
            //when the user is not yet authenticated + app.UseAuthentication() below
            services.AddAuthentication(options =>
                {
                    //Set AuthenticationScheme to cookies
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    // options.DefaultAuthenticateScheme = "Cookies";
                })
                .AddCookie()
                .AddOpenIdConnect(options => {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Authority = identityUrl.ToString();
                    options.SignedOutRedirectUri = callBackUrl.ToString();
                    options.ClientId = "mvc";
                    //secret is the private key need to changed in here and also in TokenServiceApi
                    options.ClientSecret = "secret";
                    //Hybrid authentication code + token
                    options.ResponseType = "code id_token";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    //in Dev we are using only http in prod we set it to true
                    options.RequireHttpsMetadata = false;
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("offline_access");

                    //this is optional 
                    //options.TokenValidationParameters = new TokenValidationParameters()
                    //{

                    //    NameClaimType = "name",
                    //    RoleClaimType = "role"
                    //};
                    options.Scope.Add("cart");
                    options.Scope.Add("order");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Catalog}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "defaultError",
                    template: "{controller=Error}/{action=Error}");
            });
        }
    }
}
