/* https://jasonwatmore.com/post/2020/08/13/blazor-webassembly-jwt-authentication-example-tutorial */
using PUSH_SERVER_WEB.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PUSH_SERVER_WEB.Services
{
    public interface IAuthenticationService
    {
        User? User { get; }
        Task Login(string username, string password);
        Task Logout();
    }

    public class AuthService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public User? User { get; private set; }

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
            User = await _httpService.Post<User>("/users/authenticate", new { username, password });
            await _localStorageService.SetItem("user", User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }
    }
}