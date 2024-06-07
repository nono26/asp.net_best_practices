using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Principal;

namespace SampleApp.BackEnd.Attributes;

public class ValidationFilterAttribute : ActionFilterAttribute //ActionFilterAttribute is an implementation of IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var param = context.ActionArguments.SingleOrDefault(p => p.Value is IIdentity);
        if (param.Value == null)
        {
            context.Result = new BadRequestObjectResult("Object is null");
            return;
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new UnprocessableEntityObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        throw new NotImplementedException();
    }
}