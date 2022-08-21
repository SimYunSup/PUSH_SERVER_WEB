/* https://jasonwatmore.com/post/2020/08/13/blazor-webassembly-jwt-authentication-example-tutorial */
using PUSH_SERVER_WEB.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PUSH_SERVER_WEB.Services
{
    public interface IAuthenticationService
    {
        User? User { get; }
        string? accessToken { get; set; }
        Task Login(string username, string password);
        Task Logout();
        Task Refresh();
    }

    public class AuthService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public User? User { get; private set; }
        public string? accessToken { get; set; }

        public AuthService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        ) {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Login(string username, string password)
        {
            User = await _httpService.Post<User>("/api/user/login", new { username, password });
            await _localStorageService.SetItem("refreshToken", User?.refresh_token);
            accessToken = User?.access_token;
        }

        public async Task Refresh() {
            var RefreshToken = await _localStorageService.GetItem<string>("refreshToken");
            User = await _httpService.Post<User>("/api/user/refresh", new { RefreshToken });
        }
        public async Task Logout()
        {
            User = null;
            await _httpService.Get<Object>("/api/user/logout");
            await _localStorageService.RemoveItem("refreshToken");
            await _localStorageService.RemoveItem("accessToken");
            _navigationManager.NavigateTo("login");
        }
    }
}