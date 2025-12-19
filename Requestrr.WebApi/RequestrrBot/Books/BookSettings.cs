using Requestrr.WebApi.RequestrrBot.DownloadClients;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public class BookSettings
    {
        public string Client { get; set; } = DownloadClient.Disabled;
    }
}
