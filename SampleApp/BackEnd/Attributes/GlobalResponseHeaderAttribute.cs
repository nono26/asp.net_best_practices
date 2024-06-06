
using Microsoft.AspNetCore.Mvc.Filters;

namespace SampleApp.BackEnd.Attributes;

public class GlobalResponseHeaderAttribute : ActionFilterAttribute
{
    public GlobalResponseHeaderAttribute() { }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers.Add("Global-Header", "Global-Header-Value");
        base.OnResultExecuting(context);
    }
}