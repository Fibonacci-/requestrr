namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Readarr
{
    public class ReadarrSettingsProvider
    {
        public ReadarrSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new ReadarrSettings
            {
                Hostname = settings.DownloadClients.Readarr.Hostname,
                BaseUrl = settings.DownloadClients.Readarr.BaseUrl,
                Port = (int)settings.DownloadClients.Readarr.Port,
                ApiKey = settings.DownloadClients.Readarr.ApiKey,
                Categories = settings.DownloadClients.Readarr.Categories.ToObject<ReadarrCategory[]>(),
                SearchNewRequests = settings.DownloadClients.Readarr.SearchNewRequests,
                MonitorNewRequests = settings.DownloadClients.Readarr.MonitorNewRequests,
                UseSSL = (bool)settings.DownloadClients.Readarr.UseSSL,
                Version = settings.DownloadClients.Readarr.Version
            };
        }
    }
}
