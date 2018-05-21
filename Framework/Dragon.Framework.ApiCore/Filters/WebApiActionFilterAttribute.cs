using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dragon.Framework.ApiCore.Filters
{
    /// <summary>
    /// Action拦截器
    /// </summary>
    public class WebApiActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Action执行时
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var firstErrorInfo = context.ModelState.Values.First().Errors[0];
                var errorMessage = firstErrorInfo.ErrorMessage;
                errorMessage = string.IsNullOrEmpty(errorMessage) ? firstErrorInfo.Exception.InnerException?.Message : errorMessage;
                errorMessage = string.IsNullOrEmpty(errorMessage) ? firstErrorInfo.Exception.Message : errorMessage;
                throw new ArgumentException(errorMessage);
            }
        }
    }
}