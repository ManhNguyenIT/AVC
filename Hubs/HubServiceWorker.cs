using System;
using System.Threading;
using System.Threading.Tasks;
using AVC.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AVC.Hubs
{
    public class HubServiceWorker : BackgroundService
    {
        private readonly ILogger<HubServiceWorker> _logger;
        private readonly IHubContext<HubService, IHubService> _hubContext;
        private readonly IMachineService _machineService;

        public HubServiceWorker(ILogger<HubServiceWorker> logger,
        IHubContext<HubService, IHubService> hubContext,
        IMachineService machineService)
        {
            _logger = logger;
            _hubContext = hubContext;
            _machineService = machineService;

            // _logger.LogInformation("Hub Service Worker running at: {Time}", DateTime.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // while (!stoppingToken.IsCancellationRequested)
            // {
            //     // _logger.LogInformation("Worker running at: {Time}", DateTime.Now);
            // await _hubContext.Clients.All.OnClientConnected(await _machineService.GetsAsync());
            await Task.Delay(1000);
            // }
        }
    }
}