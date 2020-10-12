using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AVC.Models;
using AVC.Interfaces;
using System.Threading.Tasks;
using System;
using AVC.DatabaseModels;
using MongoDB.Driver;

namespace AVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITotalService _totalService;
        private readonly ILogService _logService;

        public HomeController(ILogger<HomeController> logger,
        ITotalService totalService,
        ILogService logService)
        {
            _logger = logger;
            _totalService = totalService;
            _logService = logService;
        }
        public IActionResult Index() => RedirectToAction("Page");

        [HttpGet("[controller]/machines")]
        public async Task<IActionResult> ListMachinesAsync()
        {
            var date = ((DateTimeOffset)DateTime.Now.Date).ToUnixTimeSeconds();
            var totals = (await _totalService.GetsAsync(Builders<Total>.Filter.Where(i => !(i.timeCreate < date))));
            return Json(totals);
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
