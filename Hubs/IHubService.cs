using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVC.DatabaseModels;

namespace AVC.Hubs
{
    public interface IHubService
    {
        Task Update(string date, Machine machines, Log log, Total total);
        Task OnClientConnected(IEnumerable<Machine> machines);
    }
}