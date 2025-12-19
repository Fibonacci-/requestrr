using Requestrr.WebApi.Controllers.DownloadClients;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public class BookSettingsProvider
    {
        public BookSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new BookSettings
            {
                Client = settings.Books.Client
            };
        }
    }
}
