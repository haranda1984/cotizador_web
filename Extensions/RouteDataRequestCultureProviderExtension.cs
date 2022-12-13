using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace HeiLiving.Quotes.Api
{
    public class RouteDataRequestCultureProvider : RequestCultureProvider
    {
        public int IndexOfCulture;
        public int IndexofUICulture;

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            string culture = null;
            string uiCulture = null;

            if (httpContext?.Request?.Path.Value?.Split('/')?.Length < IndexOfCulture)
            {
                culture = uiCulture = httpContext.Request.Path.Value.Split('/')[IndexOfCulture]?.ToString();
            }
            else
            {
                culture = uiCulture = "es-MX";
            }

            var providerResultCulture = new ProviderCultureResult(culture, uiCulture);

            return Task.FromResult(providerResultCulture);
        }
    }
}