/* https://jasonwatmore.com/post/2020/08/13/blazor-webassembly-jwt-authentication-example-tutorial */
using PUSH_SERVER_WEB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Net;

namespace PUSH_SERVER_WEB.Helpers
{
    public class AppRouteView : RouteView
    {
        private bool isRefreshing { get; set; } = false;
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        [Inject]
        public IAuthenticationService? AuthenticationService { get; set; }
        
        [Inject]
        public ILocalStorageService? LocalStorageService { get; set; }

        protected override async void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
            var accessToken = LocalStorageService!.GetItem<string>("accessToken");
            if (accessToken == null && !isRefreshing && authorize && NavigationManager != null && AuthenticationService != null && AuthenticationService?.accessToken == null)
            {
                isRefreshing = true;
                var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
                var refreshResult = await AuthenticationService!.Refresh();
                if (refreshResult != null)
                {
                    AuthenticationService.accessToken = refreshResult?.access_token;
                    base.Render(builder);
                }
                else
                    NavigationManager.NavigateTo("/login");
            }
            else
            {
                base.Render(builder);
            }
        }
    }
}