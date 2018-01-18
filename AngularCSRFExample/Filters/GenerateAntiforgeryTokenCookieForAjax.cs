using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AngularCSRFExample.Filters
{
    public class GenerateAntiforgeryTokenCookieForAjaxAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var antiforgery = context.HttpContext.RequestServices.GetService<IAntiforgery>();

            var logger = context.HttpContext.RequestServices.GetService<ILogger<GenerateAntiforgeryTokenCookieForAjaxAttribute>>();
            logger.LogInformation("Is user authenticated?: " + context.HttpContext.User.Identities.First().IsAuthenticated);

            // We can send the request token as a JavaScript-readable cookie, and Angular will use it by default.
            var tokenName = "XSRF-TOKEN";
            var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
            context.HttpContext.Response.Cookies.Delete(tokenName);
            context.HttpContext.Response.Cookies.Append(
                tokenName,
                tokens.RequestToken,
                new CookieOptions() { HttpOnly = false, SameSite = SameSiteMode.Lax });
        }
    }
}
