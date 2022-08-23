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
        Task<T?> Get<T>(string uri);
        Task<T?> Post<T>(string uri, object value);
        Task<T?> Delete<T>(string uri);
        Task<T?> Put<T>(string uri, object value);
        Task<T?> Patch<T>(string uri, object value);
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

        public async Task<T?> Get<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request, uri);
        }

        public async Task<T?> Post<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request, uri);
        }
        public async Task<T?> Delete<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            return await sendRequest<T>(request, uri);
        }
        public async Task<T?> Put<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request, uri);
        }
        public async Task<T?> Patch<T>(string uri, object value)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, uri);
            request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return await sendRequest<T>(request, uri);
        }

        // helper methods

        private async Task<T?> sendRequest<T>(HttpRequestMessage request, string uri)
        {
            // add jwt auth header if user is logged in and request is to the api url
            var token = await _localStorageService.GetItem<string>("accessToken");
            var isApiUrl = !request.RequestUri?.IsAbsoluteUri ?? false;
            if (token != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (uri != "/api/user/refresh")
                {
                    try
                    {
                        var RefreshToken = await _localStorageService.GetItem<string>("refreshToken");

                        var Response = await Post<User>("/api/user/refresh", new { refresh_token= RefreshToken });

                        await _localStorageService.SetItem("refreshToken", Response?.refresh_token);
                        await _localStorageService.SetItem("accessToken", Response?.access_token);
                        response = await _httpClient.SendAsync(request);
                    }
                    catch
                    {
                        await _localStorageService.RemoveItem("refreshToken");
                        await _localStorageService.RemoveItem("accessToken");
                        _navigationManager.NavigateTo("/login");
                        return default;
                    }
                }
                else
                {
                    await _localStorageService.RemoveItem("refreshToken");
                    await _localStorageService.RemoveItem("accessToken");
                    _navigationManager.NavigateTo("/login");
                    return default;
                }
            }

            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (error != null)
                {
                    throw new Exception(error["error_message"]);
                }
                else
                {
                    throw new Exception();
                }
            }
            var sb = new StringBuilder();
            var body = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
                sb.AppendLine(body);
            Console.WriteLine(sb.ToString());
            if (sb.ToString() != "")
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                return default;
            }
        }
    }
}