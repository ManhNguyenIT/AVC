using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AVC.DatabaseModels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AVC.Hubs
{
    public partial class HostedService : IHubService, IHostedService
    {
        private readonly ILogger<HostedService> _logger;
        private HubConnection _connection;
        public HostedService(ILogger<HostedService> logger)
        {
            _logger = logger;

            _connection = new HubConnectionBuilder()
                .WithUrl(HubServiceConfig.HubUrl)
                .Build();

            _connection.On<string, Machine, Log, Total>(HubServiceConfig.Events.Update, Update);
            _connection.On<IEnumerable<Machine>>(HubServiceConfig.Events.OnClientConnected, OnClientConnected);
        }

        public Task Update(string date, Machine machines, Log log, Total total)
        {
            return Task.CompletedTask;
        }
        public Task OnClientConnected(IEnumerable<Machine> machines)
        {
            return Task.CompletedTask;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Loop is here to wait until the server is running
            while (true)
            {
                try
                {
                    await _connection.StartAsync(cancellationToken);
                    break;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _connection.DisposeAsync();
        }
    }
}