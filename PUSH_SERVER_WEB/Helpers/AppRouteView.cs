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

        protected override void Render(RenderTreeBuilder builder)
        {
            var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
            var path = new Uri(NavigationManager?.Uri).PathAndQuery;
            if (authorize && NavigationManager != null && AuthenticationService?.accessToken == null && path != "/login")
            {
                Console.WriteLine(path);
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                base.Render(builder);
            }
        }
    }
}