namespace Globomantics.Api
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _ApiKeyName = "XAPIKey";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            var apiKeyPresentInHeader = context.Request.Headers.TryGetValue(_ApiKeyName, out var extractedApiKey);
            var apiKey = configuration[_ApiKeyName];

            if ((apiKeyPresentInHeader && apiKey == extractedApiKey) || 
                context.Request.Path.StartsWithSegments("/swagger")) 
            {
                await _next(context);
                return;
            }
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API Key");
        }
    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKey(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
