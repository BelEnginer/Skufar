namespace Web.Middleware;

public static class ExceptionHandlerExtensions
{
    public static void AddCustomExceptionHandler(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.Configure<ProblemDetailsOptions>(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                if (env.IsDevelopment()) return;
                ctx.ProblemDetails.Extensions.Remove("exception");
                ctx.ProblemDetails.Extensions.Remove("headers");
            };
        });
    }
}