using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AVC.Models;
using AVC.Interfaces;
using AVC.DatabaseModels;
using Newtonsoft.Json;
using System.Linq;

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
        public async Task<IActionResult> Machines()
        {
            return Ok(await _machineService.GetsAsync());
        }

        [HttpGet("[controller]/types")]
        public IActionResult Types()
        {
            return Ok(System.Enum.GetValues(typeof(GPIO_TYPE))
                        .Cast<GPIO_TYPE>()
                        .Select(i => new
                        {
                            id = (int)i,
                            name = i.ToString()
                        }));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("[controller]/create-machine")]
        public async Task<IActionResult> CreateMachine(string values)
        {
            var machine = new Machine();
            JsonConvert.PopulateObject(values, machine);

            if (!TryValidateModel(machine))
                return BadRequest(ModelState.Values);

            await _machineService.CreateAsync(machine);
            return Ok();
        }

        [ValidateAntiForgeryToken]
        [HttpPut("[controller]/update-machine")]
        public async Task<IActionResult> UpdateMachine(string key, string values)
        {
            var machine = await _machineService.FindByIdAsync(key);
            if (machine == null)
            {
                return NotFound();
            }
            var _machine = JsonConvert.DeserializeObject<Machine>(values);
            machine.status = _machine.status;
            machine.gpio = _machine.gpio;
            machine.ip = _machine.ip;
            machine.name = _machine.name;

            if (!TryValidateModel(machine))
                return BadRequest(ModelState.Values);

            await _machineService.UpdateByIdAsync(key, machine);
            return Ok();
        }

        [ValidateAntiForgeryToken]
        [HttpDelete("[controller]/delete-machine")]
        public async Task<IActionResult> DeleteMachine(string key)
        {
            await _machineService.DeleteByIdAsync(key);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
