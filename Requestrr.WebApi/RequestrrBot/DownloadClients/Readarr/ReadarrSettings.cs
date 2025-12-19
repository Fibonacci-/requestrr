using System.Collections.Generic;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Lidarr;

namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Readarr
{
    public class ReadarrSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Hostname { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8787;
        public bool UseSSL { get; set; } = false;
        public string Version { get; set; } = "1";
        public ReadarrCategory[] Categories { get; set; } = new ReadarrCategory[0];
        public bool SearchNewRequests { get; set; } = true;
        public bool MonitorNewRequests { get; set; } = true;
    }

    public class ReadarrCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProfileId { get; set; }
        public int MetadataProfileId { get; set; }
        public string RootFolder { get; set; }
        public int[] Tags { get; set; } = new int[0];
    }
}
