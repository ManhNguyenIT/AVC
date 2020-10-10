﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AVC.Models;
using AVC.Interfaces;
using AVC.DatabaseModels;

namespace AVC.Controllers
{
    public class SettingController : Controller
    {
        private readonly ILogger<SettingController> _logger;
        private readonly IMachineService _machineService;
        public SettingController(ILogger<SettingController> logger,
        IMachineService machineService)
        {
            _logger = logger;
            _machineService = machineService;
        }
        public IActionResult Index() => RedirectToAction("Page");
        public IActionResult Page()
        {
            return View();
        }

        [HttpGet("[controller]/machines")]
        public async Task<IActionResult> GetMachine()
        {
            return Ok(await _machineService.GetsAsync());
        }

        [ValidateAntiForgeryToken]
        [HttpPost("[controller]/create-machine")]
        public async Task<IActionResult> CreateMachine(Machine machine)
        {
            if (!TryValidateModel(machine))
                return BadRequest(ModelState.Values);

            await _machineService.CreateAsync(machine);
            return Ok();
        }

        [ValidateAntiForgeryToken]
        [HttpPut("[controller]/update-machine")]
        public async Task<IActionResult> UpdateMachine(Machine machine)
        {
            if (!TryValidateModel(machine))
                return BadRequest(ModelState.Values);
            await _machineService.UpdateByIdAsync(machine.id, machine);
            return Ok();
        }

        [ValidateAntiForgeryToken]
        [HttpDelete("[controller]/delete-machine")]
        public async Task<IActionResult> DeleteMachine(Machine machine)
        {
            await _machineService.DeleteByIdAsync(machine.id);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
