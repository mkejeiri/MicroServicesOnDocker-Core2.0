using System.Net.Http;
using System.Threading.Tasks;

namespace MicroServicesOnDocker.Web.WebMvc.Infrastructure
{
    public interface IHttpClient
    {
        Task<string> GetTotalCatalogItems(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");
        Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string authorizationMethod = "Bearer");
        Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");
        Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string authorizationMethod = "Bearer");
    }
}