
using Microsoft.AspNetCore.Mvc;
using OperationalWorkspace.Domain.Exceptions;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception. CorrelationId: {CorrelationId}",
                correlationId);

            var problem = MapException(ex, correlationId);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status ?? 500;

            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private ProblemDetails MapException(Exception ex, string correlationId)
    {
        // ======================================================
        // DOMAIN EXCEPTIONS (YOUR CODE BASE)
        // ======================================================
        if (ex is DomainException domainEx)
        {
            return new ProblemDetails
            {
                Status = domainEx switch
                {
                    BusinessRuleException => 400,
                    CreditLimitExceededException => 409,
                    InsufficientStockException => 409,
                    _ => 400
                },

                Title = "Business rule violation",
                Detail = domainEx.Message,
                Instance = correlationId,
                Type = "https://httpstatuses.com/400"
            };
        }

        // ======================================================
        // DEFAULT SYSTEM ERROR
        // ======================================================
        return new ProblemDetails
        {
            Status = 500,
            Title = "Server error",
            Detail = _env.IsDevelopment()
                ? ex.ToString()
                : "An unexpected error occurred",
            Instance = correlationId,
            Type = "https://httpstatuses.com/500"
        };
    }
}