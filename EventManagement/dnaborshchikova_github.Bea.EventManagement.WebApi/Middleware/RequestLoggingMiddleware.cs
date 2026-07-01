namespace dnaborshchikova_github.Bea.EventManagement.WebApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private static int _counter = 0;

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var requestId = Interlocked.Increment(ref _counter);

            using (_logger.BeginScope(new Dictionary<string, object> { ["RequestId"] = $"REQ-{requestId:0000}" }))
            {
                _logger.LogInformation("HTTP {Method} {Path} START",
                    httpContext.Request.Method, httpContext.Request.Path);

                await _next(httpContext);

                _logger.LogInformation("HTTP {Method} {Path} END",
                    httpContext.Request.Method, httpContext.Request.Path);
            }
        }
    }
}
