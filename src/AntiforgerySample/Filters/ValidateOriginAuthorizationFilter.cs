using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AntiforgerySample.Filters
{
    public class ValidateOriginAuthorizationFilter : IAuthorizationFilter
    {
        private IOptions<ValidateOriginOptions> options;

        public ValidateOriginAuthorizationFilter(IOptions<ValidateOriginOptions> options)
        {
            this.options = options;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Validate only "unsafe" methods
            if (!ShouldValidate(context.HttpContext)) return;

            // Make sure we can get the request origin
            var requestOriginUrl = GetRequestOriginFromHeaders(context.HttpContext);
            if (requestOriginUrl == null)
            {
                context.Result = new BadRequestResult();
            }

            // If it is any of the whitelisted origins, allow it (This could be more complex and allow for subdomains)
            if (options.Value.WhitelistedOrigins.Contains(requestOriginUrl.Host))
            {
                return;
            }

            // Otherwise, get the target origin and make sure they match
            var targetOriginHost = GetTargetOriginFromHeaders(context.HttpContext);
            if (!targetOriginHost.Equals(requestOriginUrl.Host, StringComparison.CurrentCultureIgnoreCase))
            {
                context.Result = new BadRequestResult();
            }
        }

        private bool ShouldValidate(HttpContext httpContext)
        {
            var method = httpContext.Request.Method;
            if (string.Equals("GET", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("HEAD", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("TRACE", method, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("OPTIONS", method, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        private Uri GetRequestOriginFromHeaders(HttpContext httpContext)
        {
            // Try with the Origin header
            var requestOrigin = httpContext.Request.Headers["Origin"];

            // If not found, then try with Referer
            if (String.IsNullOrWhiteSpace(requestOrigin))
            {                
                requestOrigin = httpContext.Request.Headers["Referer"];
            }

            if (String.IsNullOrEmpty(requestOrigin)) return null;
            return new Uri(requestOrigin);
        }

        private string GetTargetOriginFromHeaders(HttpContext httpContext)
        {
            // Try with the Host header
            var targetOrigin = httpContext.Request.Headers["Host"];

            // If not found, then try with X-Forwarded-Host
            if (String.IsNullOrWhiteSpace(targetOrigin))
            {                
                targetOrigin = httpContext.Request.Headers["X-Forwarded-Host"];
            }

            // If still not found, will use the request url
            return String.IsNullOrEmpty(targetOrigin) ?
                httpContext.Request.Host.Host :
                new HostString(targetOrigin).Host;
        }
    }
}
