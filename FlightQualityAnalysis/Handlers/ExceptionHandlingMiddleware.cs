using Microsoft.Extensions.Logging;

namespace FlightQualityAnalysis.Handlers;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

/// <summary>
/// Handles exceptions that occur during the request processing pipeline.
/// Logs the exception and returns a JSON response with the error details.
/// </summary>
/// <param name="context">The HTTP context.</param>
/// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApplicationException ex)
        {
            _logger.LogError(ex, ex.Message.ToString());
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var response = new { message = "Validation Exception", details = ex.Message };
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message.ToString()); 
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new { message = "Internal Server Error", details = ex.Message };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
