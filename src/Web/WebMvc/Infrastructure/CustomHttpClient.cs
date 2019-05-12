using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MicroServicesOnDocker.Web.WebMvc.Infrastructure
{
    public class CustomHttpClient : IHttpClient
    {
        private readonly ILogger<IHttpClient> _logger;
        private readonly HttpClient _client;

        public CustomHttpClient(ILogger<IHttpClient> logger, HttpClient client)
        {
            _logger = logger;
            //_client = new HttpClient();
            _client = client;
        }
        public async Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            //set the header authorizationToken is not null
            if (authorizationToken !=null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
           
            var response = await _client.SendAsync(requestMessage);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTotalCatalogItems(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
            var response = await _client.SendAsync(requestMessage);
            return await response.Content.ReadAsStringAsync() ;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string authorizationMethod = "Bearer") => await SendHttpMessageAsync(HttpMethod.Post, uri, item, authorizationToken, authorizationMethod);

        public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri) ;
            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
            return await _client.SendAsync(requestMessage);
        } 
        public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string authorizationMethod = "Bearer") => await SendHttpMessageAsync(HttpMethod.Put, uri, item, authorizationToken, authorizationMethod);

        private async Task<HttpResponseMessage> SendHttpMessageAsync<T>(HttpMethod httpMethod, string uri, T item, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            var allowed = new List<HttpMethod> { HttpMethod.Post, HttpMethod.Put};
            if (!allowed.Contains(httpMethod))
            {
                throw new ArgumentException("Invalid httpMethod Exception", nameof(httpMethod));
            }
            var requestMessage = new HttpRequestMessage(httpMethod, uri)
            {
                Content = new StringContent(content: JsonConvert.SerializeObject(item), encoding: System.Text.Encoding.UTF8, mediaType: "application/json")
            };
            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }
            //raise an exception for http response code 500
            //needed for circuit breaker to track the failure
            var response = await _client.SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException(message: "Internal Server Error");
            }
            return response;
        }

    }
}