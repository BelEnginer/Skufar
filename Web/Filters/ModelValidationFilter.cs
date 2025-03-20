using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters;

public class ModelValidationFilter(ILoggerFactory loggerFactory) : IActionFilter
{
    private readonly ILogger<ModelValidationFilter> _logger = loggerFactory.CreateLogger<ModelValidationFilter>();
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogWarning("Received a request with a null object. Controller: {Controller}, Action: {Action}", 
            context.Controller.GetType().Name, context.RouteData.Values["action"]);

        var param = context.ActionArguments.Values.FirstOrDefault(x => x is not null);
        if (param is null)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Error = "The passed object is null.",
                Controller = context.Controller.GetType().Name,
                Action = context.RouteData.Values["action"]
            });
            return;
        }

        if (context.ModelState.IsValid) return;
        _logger.LogWarning("Invalid model state for Controller: {Controller}, Action: {Action}. ModelState: {ModelState}", 
            context.Controller.GetType().Name, context.RouteData.Values["action"], context.ModelState);
            
        context.Result = new UnprocessableEntityObjectResult(context.ModelState);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
