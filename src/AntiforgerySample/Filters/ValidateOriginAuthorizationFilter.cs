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

            // Make sure we can get the origin
            var origin = GetOriginFromHeaders(context.HttpContext);
            if (origin == null)
            {
                context.Result = new BadRequestResult();
            }

            // Validate the origin is in the whitelist
            //  This could be more complex and carefully allow subdomains
            //  You might also be behind a proxy or configure it might be painful
            //  so you could consider using the Host and X-Forwarded-Host headers instead of the whitelisted configuration
            if (!options.Value.WhitelistedOrigins.Contains(origin.Host))
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

        private Uri GetOriginFromHeaders(HttpContext httpContext)
        {
            // Try with the Origin header
            var origin = httpContext.Request.Headers["Origin"];
            if (String.IsNullOrWhiteSpace(origin))
            {
                // If not found, then try with Referer
                origin = httpContext.Request.Headers["Referer"];
            }

            if (String.IsNullOrEmpty(origin)) return null;
            return new Uri(origin);
        }
    }
}
