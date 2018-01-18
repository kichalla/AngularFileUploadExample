using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AngularCSRFExample.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AngularCSRFExample.Controllers
{
    public class TokenController : Controller
    {
        [HttpGet]
        public IActionResult Create([FromServices]IConfiguration configurationRoot)
        {
            var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.NameId, "10"),
                        new Claim(JwtRegisteredClaimNames.Sub, "James"),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, "james@james.com")
                    };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationRoot["JwtSecurityToken:Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: configurationRoot["JwtSecurityToken:Issuer"],
                audience: configurationRoot["JwtSecurityToken:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: signingCredentials
                );

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Expiration = jwtSecurityToken.ValidTo,
                Claims = jwtSecurityToken.Claims
            });
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [GenerateAntiforgeryTokenCookieForAjax]
        public IActionResult CreateCSRFToken()
        {
            return Ok();
        }
    }
}
