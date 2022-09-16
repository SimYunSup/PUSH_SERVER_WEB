/* https://jasonwatmore.com/post/2020/08/13/blazor-webassembly-jwt-authentication-example-tutorial */
using PUSH_SERVER_WEB.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PUSH_SERVER_WEB.Services
{
    public interface IAuthenticationService
    {
        string? accessToken { get; set; }
        Task Login(string username, string password);
        Task Logout();
        Task<User?> Refresh();
    }

    public class AuthService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public string? accessToken { get; set; }

        public AuthService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        ) {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            var path = new Uri(navigationManager.Uri).PathAndQuery;
            if (path != "/login" && accessToken == null)
            {
                this.Refresh();
            }
        }

        public async Task Login(string username, string password)
        {
            var User = await _httpService.Post<User>("/api/user/login", new { username, password });
            await _localStorageService.SetItem("refreshToken", User?.refresh_token);
            await _localStorageService.SetItem("accessToken", User?.access_token);
            accessToken = User?.access_token;
        }

        public async Task<User?> Refresh() {
            var RefreshToken = await _localStorageService.GetItem<string>("refreshToken");

            var Response = await _httpService.Post<User>("/api/user/refresh", new { refresh_token= RefreshToken });

            await _localStorageService.SetItem("refreshToken", Response?.refresh_token);
            await _localStorageService.SetItem("accessToken", Response?.access_token);
            accessToken = Response?.access_token;
            return Response;
        }
        public async Task Logout()
        {
            await _httpService.Get<Object>("/api/user/logout");
            await _localStorageService.RemoveItem("refreshToken");
            await _localStorageService.RemoveItem("accessToken");
            _navigationManager.NavigateTo("login");
        }
    }
}