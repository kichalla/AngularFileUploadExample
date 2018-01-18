using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngularCSRFExample.Filters;
using AngularCSRFExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularCSRFExample.Controllers
{
    public class UsersController : Controller
    {
        [GenerateAntiforgeryTokenCookieForAjax]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Create([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(user);
        }
    }
}
