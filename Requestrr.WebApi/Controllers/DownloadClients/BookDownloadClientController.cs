using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Requestrr.WebApi.config;
using Requestrr.WebApi.Controllers.DownloadClients.Readarr;
using Requestrr.WebApi.RequestrrBot.DownloadClients;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Radarr;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Sonarr;
using Requestrr.WebApi.RequestrrBot.Locale;
using Requestrr.WebApi.RequestrrBot.Movies;
using Requestrr.WebApi.RequestrrBot.Music;
using Requestrr.WebApi.RequestrrBot.Books;
using Requestrr.WebApi.RequestrrBot.TvShows;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Requestrr.WebApi.Controllers.DownloadClients
{
    [ApiController]
    [Authorize]
    [Route("/api/book")]
    public class BookDownloadClientController : ControllerBase
    {
        private readonly BookSettings _bookSettings;
        private readonly MoviesSettings _moviesSettings;
        private readonly TvShowsSettings _tvShowsSettings;
        private readonly DownloadClientsSettings _downloadClientsSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookDownloadClientController(
            IHttpClientFactory httpClientFactory,
            BookSettingsProvider bookSettingsProvider,
            MoviesSettingsProvider moviesSettingsProvider,
            TvShowsSettingsProvider tvShowsSettingsProvider,
            DownloadClientsSettingsProvider downloadClientsSettingsProvider)
        {
            _httpClientFactory = httpClientFactory;
            _bookSettings = bookSettingsProvider.Provide();
            _moviesSettings = moviesSettingsProvider.Provide();
            _tvShowsSettings = tvShowsSettingsProvider.Provide();
            _downloadClientsSettings = downloadClientsSettingsProvider.Provide();
        }


        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
            List<string> otherCategories = new List<string>();
            switch (_moviesSettings.Client)
            {
                case "Radarr":
                    foreach (RadarrCategory category in _downloadClientsSettings.Radarr.Categories)
                    {
                        otherCategories.Add(category.Name.ToLower());
                    }
                    break;
                case "Overseerr":
                    foreach (RequestrrBot.DownloadClients.Overseerr.OverseerrMovieCategory category in _downloadClientsSettings.Overseerr.Movies.Categories)
                    {
                        otherCategories.Add(category.Name.ToLower());
                    }
                    if (otherCategories.Count == 0)
                        otherCategories.Add(Language.Current.DiscordCommandMovieRequestTitleName.ToLower());
                    break;
                case "Ombi":
                    otherCategories.Add(Language.Current.DiscordCommandMovieRequestTitleName.ToLower());
                    break;
            }

            switch (_tvShowsSettings.Client)
            {
                case "Sonarr":
                    foreach (SonarrCategory category in _downloadClientsSettings.Sonarr.Categories)
                    {
                        otherCategories.Add(category.Name.ToLower());
                    }
                    break;
                case "Overseerr":
                    foreach (RequestrrBot.DownloadClients.Overseerr.OverseerrTvShowCategory category in _downloadClientsSettings.Overseerr.TvShows.Categories)
                    {
                        otherCategories.Add(category.Name.ToLower());
                    }
                    if (otherCategories.Count == 0)
                        otherCategories.Add(Language.Current.DiscordCommandTvRequestTitleName.ToLower());
                    break;
                case "Ombi":
                    otherCategories.Add(Language.Current.DiscordCommandTvRequestTitleName.ToLower());
                    break;
            }

            return Ok(new BookSettingsModel
            {
                Client = _bookSettings.Client,
                Readarr = new ReadarrSettingsModel
                {
                    Hostname = _downloadClientsSettings.Readarr.Hostname,
                    BaseUrl = _downloadClientsSettings.Readarr.BaseUrl,
                    Port = _downloadClientsSettings.Readarr.Port,
                    ApiKey = _downloadClientsSettings.Readarr.ApiKey,
                    Categories = _downloadClientsSettings.Readarr.Categories.Select(x => new ReadarrSettingsCategory
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ProfileId = x.ProfileId,
                        MetadataProfileId = x.MetadataProfileId,
                        RootFolder = x.RootFolder,
                        Tags = x.Tags
                    }).ToArray(),
                    UseSSL = _downloadClientsSettings.Readarr.UseSSL,
                    SearchNewRequests = _downloadClientsSettings.Readarr.SearchNewRequests,
                    MonitorNewRequests = _downloadClientsSettings.Readarr.MonitorNewRequests,
                    Version = _downloadClientsSettings.Readarr.Version
                },
                OtherCategories = otherCategories.ToArray()
            });
        }


        [HttpPost("disable")]
        public async Task<IActionResult> SaveAsync()
        {
            _bookSettings.Client = DownloadClient.Disabled;
            DownloadClientsSettingsRepository.SetDisabledClient(_bookSettings);
            return Ok(new { ok = true });
        }
    }
}
