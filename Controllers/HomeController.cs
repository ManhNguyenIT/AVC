using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AVC.Models;
using AVC.Interfaces;
using System.Threading.Tasks;
using System;
using AVC.DatabaseModels;
using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;

namespace AVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISummaryService _summaryService;
        private readonly IMachineService _machineService;
        private readonly ILogService _logService;

        public HomeController(ILogger<HomeController> logger,
        ISummaryService summaryService,
        IMachineService machineService,
        ILogService logService)
        {
            _logger = logger;
            _summaryService = summaryService;
            _machineService = machineService;
            _logService = logService;
        }
        public IActionResult Index() => RedirectToAction("Page");

        [HttpGet("[controller]/machines")]
        public async Task<IActionResult> Machines()
        {
            return Ok(await _machineService.GetsAsync());
        }

        [HttpGet("[controller]/summaries")]
        public async Task<IActionResult> Summaries()
        {
            IEnumerable<Summary> summaries = new List<Summary>();
            try
            {
                var date = ((DateTimeOffset)DateTime.Now.Date.AddDays(-7)).ToUnixTimeSeconds();
                summaries = await _summaryService.GetsAsync(Builders<Summary>.Filter.Where(i => !(i.timeCreate < date)),
                                                                                        new FindOptions<Summary, Summary>() { Sort = Builders<Summary>.Sort.Ascending(i => i.timeCreate) });

                date = ((DateTimeOffset)DateTime.Now.Date).ToUnixTimeSeconds();
                foreach (var summary in summaries)
                {
                    if (!(summary.timeCreate < date))
                    {
                        var machine = await _machineService.FindByIpAsync(summary.ip);
                        var index = machine?.gpio.FindIndex(i => i.type == GPIO_TYPE.TIMER) ?? -1;
                        if (index != -1)
                        {
                            var _log = (await _logService
                                        .GetsAsync(Builders<Log>.Filter.Where(i => i.gpio.value == 0 && i.ip == machine.ip && i.gpio.port == machine.gpio[index].port && !(i.timeCreate < date)),
                                                    new FindOptions<Log, Log>() { Limit = 1, Sort = Builders<Log>.Sort.Descending(i => i.timeCreate) }))
                                        .FirstOrDefault();
                            if (_log == null)
                            {
                                _log = new Log() { timeCreate = new DateTimeOffset(DateTime.Now.Date.AddHours(7)).ToUnixTimeSeconds() };
                            }
                            summary._time += (((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds() - _log.timeCreate);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError(e.Message);
            }
            return Json(summaries);
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
