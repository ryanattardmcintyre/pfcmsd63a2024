using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index([FromServices] RedisRepository rr, [FromServices] LogsRepository lr)
        {
            try
            {
                lr.WriteLogEntry("HomeController-Log", "About to access the redis cache to increment the counter");
                rr.IncrementCounter();
                lr.WriteLogEntry("HomeController-Log", "Incremented the counter by one");

                throw new Exception("Exception thrown on purpose");
            }
            catch (Exception ex)
            {
                lr.WriteLogEntry("HomeController-Log", ex.Message, Google.Cloud.Logging.Type.LogSeverity.Error);

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize] //allows access to this method only to logged in users (i.e. members)
        public IActionResult MembersLanding()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); //erases the cookie which holds the user logged in
            return RedirectToAction("Index"); //redirects the user to the home page (which is another method in this same controller)

            
        }
    }
}