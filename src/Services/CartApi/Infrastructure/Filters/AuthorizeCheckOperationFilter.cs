using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MicroServicesOnDocker.Services.CartApi.Infrastructure.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        //Operation will observe here the controller and actions,
        //once an authorized attribute is found send Unauthorized/Forbidden response
        public void Apply(Operation operation, OperationFilterContext context)
        {
            //find anything with authorize attribute, if not return!
            if (!context.ApiDescription.TryGetMethodInfo(out var mi)) return;
            if (!mi.GetCustomAttributes().OfType<AuthorizeAttribute>().Any()) return;

            //once an authorize attribute is found!
            operation.Responses.Add("401", new Response { Description = "Unauthorized" });
            operation.Responses.Add("403", new Response { Description = "Forbidden" });

            operation.Security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>>
                {
                    //this would give the OAuth2Scheme name + scope --> see startup class
                    { "oauth2", new [] { "cart" } }
                }
            };
        }
    }
}
