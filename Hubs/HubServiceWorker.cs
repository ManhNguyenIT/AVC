using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AVC.DatabaseModels;
using AVC.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AVC.Hubs
{
    public class HubServiceWorker : BackgroundService
    {
        private readonly ILogger<HubServiceWorker> _logger;
        private readonly IHubContext<HubService, IHubService> _hubContext;
        private readonly ILogService _logService;
        private readonly ISummaryService _summaryService;
        private readonly IMachineService _machineService;

        public HubServiceWorker(ILogger<HubServiceWorker> logger,
        IHubContext<HubService, IHubService> hubContext,
        ISummaryService summaryService,
        ILogService logService,
        IMachineService machineService)
        {
            _logger = logger;
            _hubContext = hubContext;
            _logService = logService;
            _summaryService = summaryService;
            _machineService = machineService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var date = ((DateTimeOffset)DateTime.Now.Date.AddDays(-7)).ToUnixTimeSeconds();
                    var summaries = await _summaryService.GetsAsync(Builders<Summary>.Filter.Where(i => !(i.timeCreate < date)),
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
                    await _hubContext.Clients.All.Summaries(summaries);
                }
                catch (System.Exception e)
                {
                    _logger.LogError(e.Message);
                }
                await Task.Delay(5000);
            }
        }
    }
}