using Data.Identity.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Middlewares
{
    public class MissingTenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _missingTenantUrl;

        public MissingTenantMiddleware(RequestDelegate next, string missingTenantUrl)
        {
            _next = next;
            _missingTenantUrl = missingTenantUrl;
        }

        public async Task Invoke(HttpContext httpContext, SignInManager<IdentityWebUser> signInManager)
        {
            var user = httpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                var identity = user.Identity as ClaimsIdentity;

                var claim = identity.Claims.FirstOrDefault(p => p.Type == "TenantId");

                if (claim == null)
                {
                    await signInManager.SignOutAsync();

                    httpContext.Response.Redirect(_missingTenantUrl);
                    return;
                }
            }

            await _next.Invoke(httpContext);
        }
    }
}
