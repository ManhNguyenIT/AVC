using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AVC.Models;
using AVC.Interfaces;
using System.Threading.Tasks;

namespace AVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILogService _logService;

        public HomeController(ILogger<HomeController> logger,
        ILogService logService)
        {
            _logger = logger;
            _logService = logService;
        }
        public IActionResult Index() => RedirectToAction("Page");

        [HttpGet("logs")]
        public async Task<IActionResult> ListLogsAsync()
        {
            return Json(await _logService.GetsAsync());
        }

        public IActionResult Page()
        {
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
    }
}
