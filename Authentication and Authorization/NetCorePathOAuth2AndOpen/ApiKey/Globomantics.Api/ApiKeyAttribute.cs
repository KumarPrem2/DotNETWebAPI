using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Globomantics.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiKeyAttribute : Attribute, IAuthorizationFilter
    {
        public const string _ApiKeyName = "XApiKey";
        private readonly IConfiguration configuration;
        public ApiKeyAttribute(IConfiguration configuration)
        {

            this.configuration = configuration;

        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var apiKeyPresentInHeader = httpContext.Request.Headers.TryGetValue(_ApiKeyName, out var extractedApiKey);
            var apiKey = configuration[_ApiKeyName];

            if ((apiKeyPresentInHeader && apiKey == extractedApiKey) ||
                httpContext.Request.Path.StartsWithSegments("/swagger"))
            {
               
                return;
            }
            context.Result = new UnauthorizedResult();
        }
    }
}
