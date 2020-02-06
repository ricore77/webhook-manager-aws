using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookManager.Helpers;

namespace WebhookManager.Api.Extensions
{
  

    public static class WebhookManagerErrorHandlingConfig
    {
        public static void AddErrorHandling(this MvcOptions options, ExceptionFilterAttribute exceptionFilterAttribute)
        {
            options.Filters.Add(exceptionFilterAttribute);
        }
    }

    public class WebhookManagerExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ApiError apiError = null;
            if (context.Exception is WebhookManagerException)
            {
                var ex = context.Exception as WebhookManagerException;
                context.Exception = null;
                apiError = new ApiError(ex.Message);
                apiError.errors = ex.Errors;
                context.HttpContext.Response.StatusCode = ex.StatusCode;
                context.Result = new JsonResult(apiError);
            }

            base.OnException(context);
        }
    }
}
