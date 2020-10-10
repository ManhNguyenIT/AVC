using System;
using System.Linq;
using System.Threading.Tasks;
using AVC.DatabaseModels;
using AVC.Hubs;
using AVC.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

[Route("service-center")]
[ApiController]
public class ServiceCenterController : ControllerBase
{
    private readonly ILogger<ServiceCenterController> _logger;
    private readonly IHubContext<HubService, IHubService> _hubContext;
    private readonly IActionContextAccessor _accessor;
    private readonly IMachineService _machineService;
    private readonly ILogService _logService;
    private readonly string ip;

    public ServiceCenterController(ILogger<ServiceCenterController> logger,
            IHubContext<HubService, IHubService> hubContext,
            IActionContextAccessor accessor,
            IMachineService machineService,
            ILogService logService)
    {
        _logger = logger;
        _hubContext = hubContext;
        _accessor = accessor;
        _machineService = machineService;
        _logService = logService;

        // System.Net.IPAddress remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
        ip = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
    }

    [HttpGet("current-time")]
    public IActionResult GetTime()
    {
        var time = DateTime.Now;
        return new JsonResult(new { time.Year, time.Month, date = time.ToShortDateString(), time.Hour, time.Minute, time.Second });
    }


    [HttpGet("init")]
    public async Task<IActionResult> UpdateMachineAsync()
    {
        var machine = await _machineService.FindByIpAsync(ip);
        if (machine == null)
        {
            return NotFound();
        }

        var date = ((DateTimeOffset)DateTime.Now.Date).ToUnixTimeSeconds();
        var logs = await _logService.GetsAsync(Builders<Log>.Filter.Where(i => i.value == 0 && i.ip == machine.ip && i.gpio == machine.gpio && i.timeCreate == date));
        var total = new Total()
        {
            ip = machine.ip,
            name = machine.name,
            gpio = machine.gpio,
            totalTime = logs.Sum(i => i.finish - i.start),
        };
        await _hubContext.Clients.Group(ip).Update(DateTime.Now.Date.ToShortDateString(), machine, logs.LastOrDefault(), total);
        return Ok();
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateMachineAsync(object values)
    {
        var machine = await _machineService.FindByIpAsync(ip);
        if (machine == null)
        {
            return NotFound();
        }

        var model = JsonConvert.DeserializeObject<Machine>(values.ToString());
        if (model.value != 0 && model.value != 1)
        {
            return BadRequest();
        }
        if (machine.value == model.value)
        {
            return Ok();
        }

        JsonConvert.PopulateObject(values.ToString(), machine);
        if (!TryValidateModel(machine))
            return BadRequest(ModelState.Values);

        var log = new Log()
        {
            gpio = machine.gpio,
            ip = machine.ip,
            name = machine.name,
            start = machine.timeUpdate,
            finish = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds(),
            value = machine.value
        };

        machine.status = true;
        if (machine.status && machine.value == 0)
        {
            machine.totalTime += log.finish - log.start;
        }
        machine.timeUpdate = log.finish;
        await _machineService.UpdateByIdAsync(machine.id, machine);

        await _logService.CreateAsync(log);

        var date = ((DateTimeOffset)DateTime.Now.Date).ToUnixTimeSeconds();
        var logs = await _logService.GetsAsync(Builders<Log>.Filter.Where(i => i.value == 0 && i.ip == machine.ip && i.gpio == machine.gpio && i.timeCreate == date));
        var total = new Total()
        {
            ip = machine.ip,
            name = machine.name,
            gpio = machine.gpio,
            totalTime = logs.Sum(i => i.finish - i.start),
        };
        await _hubContext.Clients.Group(ip).Update(DateTime.Now.Date.ToShortDateString(), machine, log, total);
        return Ok();
    }
}