using DigitizedDallet.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DigitizedDallet;

public class ViewBagActionFilter : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {   
        if (context.Controller is Controller controller)
        {
            controller.ViewBag.MyProperty = "value";

            controller.ViewBag.RequestedLanguage = context.HttpContext.Request.HttpContext.GetUserCulture() ?? "en";
            controller.ViewBag.RequestedScript = context.HttpContext.Request.RouteValues["script"]?.ToString() ?? "Latn";
        }

        base.OnResultExecuting(context);
    }
}
