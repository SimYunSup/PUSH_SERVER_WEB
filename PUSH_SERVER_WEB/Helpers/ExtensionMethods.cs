/* https://jasonwatmore.com/post/2020/08/13/blazor-webassembly-jwt-authentication-example-tutorial */
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Specialized;
using System.Web;

namespace PUSH_SERVER_WEB.Helpers
{
    public static class ExtensionMethods
    {
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string? QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }
    }
}