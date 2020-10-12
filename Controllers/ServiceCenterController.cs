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
    private readonly ITotalService _totalService;
    private readonly ILogService _logService;
    private readonly string ip;

    public ServiceCenterController(ILogger<ServiceCenterController> logger,
            IHubContext<HubService, IHubService> hubContext,
            IActionContextAccessor accessor,
            IMachineService machineService,
            ITotalService totalService,
            ILogService logService)
    {
        _logger = logger;
        _hubContext = hubContext;
        _accessor = accessor;
        _machineService = machineService;
        _totalService = totalService;
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

    [HttpPost("update")]
    public async Task<IActionResult> UpdateMachineAsync([FromBody] GPIO gpio)
    {
        var machine = await _machineService.FindByIpAsync(ip);
        if (machine == null)
        {
            return NotFound();
        }

        if (!TryValidateModel(gpio))
            return BadRequest(ModelState.Values);

        var index = machine.gpio.FindIndex(i => gpio.port == gpio.port);
        if (gpio.value < 0 || gpio.value > 1 || index < 0)
        {
            return BadRequest();
        }

        if (machine.gpio[index].value == gpio.value)
        {
            return Ok();
        }
        else
        {
            var log = new Log()
            {
                machine = machine,
                start = machine.timeUpdate,
                finish = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds(),
            };
            await _logService.CreateAsync(log);

            machine.status = true;
            machine.gpio[index].value = gpio.value;
            machine.timeUpdate = log.finish;
            var date = ((DateTimeOffset)DateTime.Now.Date).ToUnixTimeSeconds();
            var total = (await _totalService.GetsAsync(Builders<Total>.Filter.Where(i => !(i.timeCreate < date) && i.machine.id == machine.id))).FirstOrDefault();

            if (total == null)
            {
                total = new Total()
                {
                    machine = machine,
                    date = DateTime.Now.Date.ToShortDateString()
                };
                if (gpio.value == 0)
                {
                    total._totalON += log.finish - log.start;
                    machine._totalON += log.finish - log.start;
                }
                else
                {
                    total._totalOFF += log.finish - log.start;
                    machine._totalOFF += log.finish - log.start;
                }
                await _totalService.CreateAsync(total);
            }
            else
            {
                if (gpio.value == 0)
                {
                    total._totalON += log.finish - log.start;
                    machine._totalON += log.finish - log.start;
                }
                else
                {
                    total._totalOFF += log.finish - log.start;
                    machine._totalOFF += log.finish - log.start;
                }
                total.machine = machine;
                await _totalService.UpdateByIdAsync(total.id, total);
            }
            await _machineService.UpdateByIdAsync(machine.id, machine);
            await _hubContext.Clients.Group(ip).Update(DateTime.Now.Date.ToShortDateString(), machine, log, total);
        }
        return Ok();
    }
}