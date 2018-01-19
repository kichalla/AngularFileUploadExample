using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AngularCSRFExample.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AngularCSRFExample.Controllers
{
    public class AuthController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "kichalla", ClaimValueTypes.String, "http://test.com"));
            var userIdentity = new ClaimsIdentity("kichalla-identity");
            userIdentity.AddClaims(claims);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

            return RedirectToAction(nameof(RefreshAntiforgeryRequestToken));
        }

        [HttpGet]
        public void RefreshAntiforgeryRequestToken([FromServices] IAntiforgery antiforgery)
        {
            // We can send the request token as a JavaScript-readable cookie, and Angular will use it by default.
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append(
                "XSRF-TOKEN",
                tokens.RequestToken,
                new CookieOptions() { HttpOnly = false, SameSite = SameSiteMode.Lax });
        }

        [HttpGet]
        public async Task<IActionResult> Logout([FromServices] IOptions<AntiforgeryOptions> antiforgeryOptions)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var antiforgeryCookieName = antiforgeryOptions.Value.Cookie.Name;
            HttpContext.Response.Cookies.Delete(antiforgeryCookieName);
            HttpContext.Response.Cookies.Delete("XSRF-TOKEN");

            return Ok();
        }
    }
}
