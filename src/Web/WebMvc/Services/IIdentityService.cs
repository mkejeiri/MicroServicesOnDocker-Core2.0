using System.Security.Principal;

namespace MicroServicesOnDocker.Web.WebMvc.Services
{
    public interface IIdentityService<T>
    {
        T Get(IPrincipal principal);
    }
}
