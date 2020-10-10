using System;
using System.Threading.Tasks;

namespace AVC.Hubs
{
    public static class HubServiceConfig
    {
        public static string HubUrl => "/hubs/live";

        public static class Events
        {
            public static string Update => nameof(IHubService.Update);
            public static string OnClientConnected => nameof(IHubService.OnClientConnected);
        }
    }
}