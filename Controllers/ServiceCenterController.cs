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

[Route("service-center")]
[ApiController]
public class ServiceCenterController : ControllerBase
{
    private readonly ILogger<ServiceCenterController> _logger;
    private readonly IHubContext<HubService, IHubService> _hubContext;
    private readonly IActionContextAccessor _accessor;
    private readonly IMachineService _machineService;
    private readonly ISummaryService _summaryService;
    private readonly ILogService _logService;
    private readonly string ip;

    public ServiceCenterController(ILogger<ServiceCenterController> logger,
            IHubContext<HubService, IHubService> hubContext,
            IActionContextAccessor accessor,
            IMachineService machineService,
            ISummaryService summaryService,
            ILogService logService)
    {
        _logger = logger;
        _hubContext = hubContext;
        _accessor = accessor;
        _machineService = machineService;
        _summaryService = summaryService;
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
    public async Task<IActionResult> Update([FromBody] GPIO gpio)
    {
        var machine = await _machineService.FindByIpAsync(ip);
        if (machine == null)
        {
            return NotFound();
        }

        if (!TryValidateModel(gpio))
            return BadRequest(ModelState.Values);

        var index = machine.gpio.FindIndex(i => i.port == gpio.port);
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
            machine.status = true;
            machine.gpio[index].value = gpio.value;
            var log = new Log()
            {
                ip = machine.ip,
                gpio = machine.gpio[index],
            };
            await _logService.CreateAsync(log);
            await _hubContext.Clients.All.Log(log);

            machine.timeUpdate = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            var date = ((DateTimeOffset)DateTime.Now.Date).ToUnixTimeSeconds();
            var summary = (await _summaryService
                            .GetsAsync(Builders<Summary>.Filter.Where(i => i.ip == machine.ip && !(i.timeCreate < date)),
                                        new FindOptions<Summary, Summary>() { Limit = 1, Sort = Builders<Summary>.Sort.Descending(i => i.timeCreate) }))
                            .FirstOrDefault();

            if (summary == null)
            {
                summary = new Summary()
                {
                    ip = machine.ip,
                    name = machine.name
                };
                await _summaryService.CreateAsync(summary);
            }

            switch (machine.gpio[index].type)
            {
                case GPIO_TYPE.COUTER:
                    if (machine.gpio[index].value == 0)
                    {
                        summary.count++;
                    }
                    break;
                case GPIO_TYPE.TIMER:
                    if (machine.gpio[index].value == 1)
                    {
                        var _log = (await _logService
                            .GetsAsync(Builders<Log>.Filter.Where(i => i.gpio.value == 0 && i.ip == machine.ip && i.gpio.port == machine.gpio[index].port && !(i.timeCreate < date)),
                                        new FindOptions<Log, Log>() { Limit = 1, Sort = Builders<Log>.Sort.Descending(i => i.timeCreate) }))
                            .FirstOrDefault();
                        if (_log == null)
                        {
                            _log = new Log() { timeCreate = new DateTimeOffset(DateTime.Now.Date.AddHours(7)).ToUnixTimeSeconds() };
                        }

                        summary._time += (log.timeCreate - _log.timeCreate);
                    }
                    break;
                default:
                    break;
            }
            await _summaryService.UpdateByIdAsync(summary.id, summary);
            await _machineService.UpdateByIdAsync(machine.id, machine);

            try
            {
                date = ((DateTimeOffset)DateTime.Now.Date.AddDays(-7)).ToUnixTimeSeconds();
                var summaries = await _summaryService.GetsAsync(Builders<Summary>.Filter.Where(i => !(i.timeCreate < date)),
                                                                                        new FindOptions<Summary, Summary>() { Sort = Builders<Summary>.Sort.Ascending(i => i.timeCreate) });

                date = ((DateTimeOffset)DateTime.Now.Date).ToUnixTimeSeconds();
                foreach (var _summary in summaries)
                {
                    if (!(_summary.timeCreate < date))
                    {
                        var _machine = await _machineService.FindByIpAsync(_summary.ip);
                        var _index = _machine?.gpio.FindIndex(i => i.type == GPIO_TYPE.TIMER) ?? -1;
                        if (_index != -1)
                        {
                            var _log = (await _logService
                                        .GetsAsync(Builders<Log>.Filter.Where(i => i.gpio.value == 0 && i.ip == _machine.ip && i.gpio.port == _machine.gpio[_index].port && !(i.timeCreate < date)),
                                                    new FindOptions<Log, Log>() { Limit = 1, Sort = Builders<Log>.Sort.Descending(i => i.timeCreate) }))
                                        .FirstOrDefault();
                            if (_log == null)
                            {
                                _log = new Log() { timeCreate = new DateTimeOffset(DateTime.Now.Date.AddHours(7)).ToUnixTimeSeconds() };
                            }
                            _summary._time += (((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds() - _log.timeCreate);
                        }
                    }
                }
                await _hubContext.Clients.All.Summaries(summaries);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
        return Ok();
    }
}