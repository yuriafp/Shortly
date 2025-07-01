using System.Net.Mime;

namespace Shortly.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error: {Message}", ex.Message);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";


                var errorResponse = new
                {
                    StatusCodes = StatusCodes.Status500InternalServerError,
                    Message = "Something failed. Try again later."
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
