
using Microsoft.AspNetCore.Mvc.Filters;

namespace SampleApp.BackEnd.Attributes;

public class GlobalResponseHeaderAttribute : ActionFilterAttribute
{
    private readonly string _headerName;
    private readonly string _headerValue;
    public GlobalResponseHeaderAttribute(string headerName, string headerValue) =>
        (_headerName, _headerValue) = (headerName, headerValue);

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers.Add(_headerName, _headerValue);
        base.OnResultExecuting(context);
    }
}