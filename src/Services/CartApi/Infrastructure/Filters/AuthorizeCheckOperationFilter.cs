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
            var found = false;
            if (context.ApiDescription.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor controller)
            {
                found = controller.ControllerTypeInfo.GetCustomAttributes().OfType<AuthorizeAttribute>().Any();
            }
            if (!found)
            {
                if (!context.ApiDescription.TryGetMethodInfo(out var mi)) return;
                if (!mi.GetCustomAttributes().OfType<AuthorizeAttribute>().Any()) return;
            }
            //once an authorize attribute is found!
            operation.Responses.TryAdd("401", new Response { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new Response { Description = "Forbidden" });

            operation.Security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>>
                {
                    //this would give the OAuth2Scheme name ("oauth2") + scope ("cart" ) --> see startup class AddSecurityDefinition
                    { "oauth2", new [] { "cart" } }
                }
            };
        }
    }
}
