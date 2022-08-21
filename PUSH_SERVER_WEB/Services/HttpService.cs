using PUSH_SERVER_WEB.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;

namespace PUSH_SERVER_WEB.Services
{
    public interface IHttpService
    {
        string accessToken { get; set; }
        Task<T?> Get<T>(string uri);
        Task<T?> Post<T>(string uri, object value);
    }

    public class HttpService : IHttpService
    {
        private HttpClient _httpClient;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private IConfiguration _configuration;
        private IJSRuntime _runtime;

        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IJSRuntime jSRuntime,
            IConfiguration configuration
        ) {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _configuration = configuration;
            _runtime = jSRuntime;
        }

        public string accessToken { get; set; }
        public async Task<T?> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request);
        }

        public async Task<T?> Post<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request);
        }

        // helper methods

        private async Task<T?> sendRequest<T>(HttpRequestMessage request)
        {
            // add jwt auth header if user is logged in and request is to the api url
            var token = accessToken;
            var isApiUrl = !request.RequestUri?.IsAbsoluteUri ?? false;
            if (token != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            
            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorageService.RemoveItem("accessToken");
                return default;
            }

            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (error != null)
                {
                    throw new Exception(error["message"]);
                }
                else
                {
                    throw new Exception();
                }
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}