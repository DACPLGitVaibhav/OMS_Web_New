using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Services.Services.LicenseValidator;

namespace OMS_Template.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LicenseMiddleware
    {
        private readonly RequestDelegate _next;

        public LicenseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!LicenseState.IsValid &&
                       !httpContext.Request.Path.StartsWithSegments("/Account/LoginPage"))
            {
                httpContext.Response.Redirect("/Account/LoginPage");
                return;
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LicenseMiddlewareExtensions
    {
        public static IApplicationBuilder UseLicenseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LicenseMiddleware>();
        }
    }
}
