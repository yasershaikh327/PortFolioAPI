using DataAccess.Helper;

namespace PortFolioAPI.GlobalExceptionMiddleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHelper _helper;

        private static readonly HashSet<int> HandledCodes = new() { 404, 500, 429, 503 };

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHelper helper)
        {
            _next = next;
            _logger = logger;
            _helper = helper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (!context.Response.HasStarted && HandledCodes.Contains(context.Response.StatusCode))
                {
                    var path = context.Request.Path.Value ?? string.Empty;

                    if (!path.StartsWith("/Error", StringComparison.OrdinalIgnoreCase))
                    {
                        var statusCode = context.Response.StatusCode;
                        context.Response.Clear();
                        context.Response.StatusCode = statusCode;
                        context.Request.Path = "/Error/Error";
                        context.Request.QueryString = new QueryString($"?statusCode={statusCode}");
                        await _next(context);
                    }
                }


            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 500;
                    context.Request.Path = "/Error/Error";
                    context.Request.QueryString = new QueryString("?statusCode=500");
                    await _next(context);
                    _helper.LogError("An error occurred while processing the request.", ex);
                }
            }
        }
    }
}
