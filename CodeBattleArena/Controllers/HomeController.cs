using CodeBattleArena.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CodeBattleArena.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult HomePage()
        {
            return View();
        }
        public IActionResult Info()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangeTheme(string theme)
        {
            if (string.IsNullOrEmpty(theme))
            {
                return BadRequest("Theme parameter is required.");
            }

            CookieOptions cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("theme", theme, cookie);

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
