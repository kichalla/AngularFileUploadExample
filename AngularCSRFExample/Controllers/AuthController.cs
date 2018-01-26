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

            if (loginModel.Email.ToLowerInvariant() == "foo@bar.com"
                && loginModel.Password == "test")
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, "foo", ClaimValueTypes.String, "http://foo.com"));
                claims.Add(new Claim(ClaimTypes.Email, "foo@bar.com", ClaimValueTypes.String, "http://foo.com"));
                var userIdentity = new ClaimsIdentity("foo-identity");
                userIdentity.AddClaims(claims);
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                // This will set the cookie(having this logged in user's claims) in the outgoing response
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                return RedirectToAction(nameof(RefreshAntiforgeryRequestToken));
            }

            return Unauthorized();
        }

        [HttpGet]
        public void RefreshAntiforgeryRequestToken([FromServices] IAntiforgery antiforgery)
        {
            // We can send the request token as a JavaScript-readable cookie, and Angular will use it by default.
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append(
                "XSRF-TOKEN",
                tokens.RequestToken,
                new CookieOptions()
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax,
                    Secure = true
                });
        }

        [HttpGet]
        public async Task<IActionResult> Logout([FromServices] IOptions<AntiforgeryOptions> antiforgeryOptions)
        {
            // Remove the session cookie from the outgoing response
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Remove the Antiforgery token cookies associated with the logged out user
            var antiforgeryCookieName = antiforgeryOptions.Value.Cookie.Name;
            HttpContext.Response.Cookies.Delete(antiforgeryCookieName);
            HttpContext.Response.Cookies.Delete("XSRF-TOKEN");

            // Refresh the antifrogery token again so that the login form can use antiforgery tokens which are not
            // associated with any identity.
            return Redirect(nameof(RefreshAntiforgeryRequestToken));
        }
    }
}
