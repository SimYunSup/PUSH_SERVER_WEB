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
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        [Inject]
        public IAuthenticationService? AuthenticationService { get; set; }

        protected override async void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
            if (authorize && NavigationManager != null && AuthenticationService != null && AuthenticationService?.User == null)
            {
                var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
                await AuthenticationService!.Refresh();
                NavigationManager.NavigateTo($"login?returnUrl={returnUrl}");
            }
            else
            {
                base.Render(builder);
            }
        }
    }
}