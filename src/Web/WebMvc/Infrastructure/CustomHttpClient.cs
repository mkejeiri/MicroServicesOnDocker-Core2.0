using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MicroServicesOnDocker.Services.WebMvc.Infrastructure
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
        public async Task<string> GetStringAsync(string uri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _client.SendAsync(requestMessage);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTotalCatalogItems(string uri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _client.SendAsync(requestMessage);
            return await response.Content.ReadAsStringAsync() ;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item) => await SendHttpMessageAsync(HttpMethod.Post, uri, item);
        public async Task<HttpResponseMessage> DeleteAsync<T>(string uri) => await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri));
        public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item) => await SendHttpMessageAsync(HttpMethod.Put, uri, item);
        private async Task<HttpResponseMessage> SendHttpMessageAsync<T>(HttpMethod httpMethod, string uri, T item)
        {
            var allowed = new List<HttpMethod> { HttpMethod.Post, HttpMethod.Put};
            if (!allowed.Contains(httpMethod))
            {
                throw new ArgumentException("Invalid httpMethod Exception", nameof(httpMethod));
            }
            var requestMessage = new HttpRequestMessage(httpMethod, uri)
            {
                Content = new StringContent(content:JsonConvert.SerializeObject(item), encoding: System.Text.Encoding.UTF8, mediaType: "application/json")
            };

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