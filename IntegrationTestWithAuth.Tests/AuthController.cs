using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationTestWithAuth.Tests
{
    [Route("test/getauth")]
    public class AuthController : Controller
    {
        // GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[]
            {
                new Claim("name","foobar"), 
                new Claim("email","info@der-albert.com"), 
            }, "Unit Test","name","role");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Content("");
        }
    }
}