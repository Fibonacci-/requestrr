namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Readarr
{
    public class ReadarrSettingsProvider
    {
        public ReadarrSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new ReadarrSettings
            {
                BaseUrl = settings.DownloadClients.Readarr.BaseUrl,
                ApiKey = settings.DownloadClients.Readarr.ApiKey,
                Hostname = settings.DownloadClients.Readarr.Hostname,
                Port = settings.DownloadClients.Readarr.Port,
                UseSSL = settings.DownloadClients.Readarr.UseSSL,
                Version = settings.DownloadClients.Readarr.Version,
                SearchNewRequests = settings.DownloadClients.Readarr.SearchNewRequests,
                MonitorNewRequests = settings.DownloadClients.Readarr.MonitorNewRequests,
                Categories = System.Array.ConvertAll<dynamic, ReadarrCategory>(settings.DownloadClients.Readarr.Categories.ToObject<dynamic[]>(), new System.Converter<dynamic, ReadarrCategory>(x => new ReadarrCategory
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProfileId = x.ProfileId,
                    MetadataProfileId = x.MetadataProfileId,
                    RootFolder = x.RootFolder,
                    Tags = System.Array.ConvertAll<dynamic, string>(x.Tags.ToObject<dynamic[]>(), new System.Converter<dynamic, string>(t => (string)t)),
                }))
            };
        }
    }
}
