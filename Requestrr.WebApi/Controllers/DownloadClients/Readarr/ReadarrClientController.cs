using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Requestrr.WebApi.RequestrrBot.DownloadClients;
using Requestrr.WebApi.RequestrrBot.DownloadClients.Readarr;
using Requestrr.WebApi.RequestrrBot.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Requestrr.WebApi.Controllers.DownloadClients.Readarr
{
    [ApiController]
    [Authorize]
    [Route("/api/book/readarr")]
    public class ReadarrClientController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReadarrClient> _logger;

        public ReadarrClientController(IHttpClientFactory httpClientFactory, ILogger<ReadarrClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestReadarrSettings([FromBody] TestReadarrSettingsModel model)
        {
            try
            {
                await ReadarrClient.TestConnectionAsync(_httpClientFactory.CreateClient(), _logger, ConvertToReadarrSettings(model));
                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("rootpath")]
        public async Task<IActionResult> GetReadarrRootPaths([FromBody] TestReadarrSettingsModel model)
        {
            try
            {
                IList<ReadarrClient.JSONRootPath> paths = await ReadarrClient.GetRootPaths(_httpClientFactory.CreateClient(), _logger, ConvertToReadarrSettings(model));
                return Ok(paths.Select(x => new ReadarrPath
                {
                    Id = x.id,
                    Path = x.path
                }));
            }
            catch (Exception)
            {
                return BadRequest($"Could not load the paths from Readarr, check your settings.");
            }
        }

        [HttpPost("profile")]
        public async Task<IActionResult> GetReadarrProfiles([FromBody] TestReadarrSettingsModel model)
        {
            try
            {
                IList<ReadarrClient.JSONProfile> profiles = await ReadarrClient.GetProfiles(_httpClientFactory.CreateClient(), _logger, ConvertToReadarrSettings(model));
                return Ok(profiles.Select(x => new ReadarrProfile
                {
                    Id = x.id,
                    Name = x.name
                }));
            }
            catch (Exception)
            {
                return BadRequest($"Could not load the profiles from Readarr, check your settings.");
            }
        }

        [HttpPost("metadataprofile")]
        public async Task<IActionResult> GetReadarrMetadatProfile([FromBody] TestReadarrSettingsModel model)
        {
            try
            {
                IList<ReadarrClient.JSONProfile> metadataProfiles = await ReadarrClient.GetMetadataProfiles(_httpClientFactory.CreateClient(), _logger, ConvertToReadarrSettings(model));
                return Ok(metadataProfiles.Select(x => new ReadarrProfile
                {
                    Id = x.id,
                    Name = x.name
                }));
            }
            catch (Exception)
            {
                return BadRequest($"Could not load metadata profiles from Readarr, check your settings.");
            }
        }

        [HttpPost("tag")]
        public async Task<IActionResult> GetReadarrTags([FromBody] TestReadarrSettingsModel model)
        {
            try
            {
                IList<ReadarrClient.JSONTag> tags = await ReadarrClient.GetTags(_httpClientFactory.CreateClient(), _logger, ConvertToReadarrSettings(model));
                return Ok(tags.Select(x => new ReadarrTag
                {
                    Id = x.id,
                    Name = x.label
                }));
            }
            catch (Exception)
            {
                return BadRequest($"Could not load the tags from Readarr, check your settings.");
            }
        }

        [HttpPost()]
        public async Task<IActionResult> SaveAsync([FromBody] SaveReadarrSettingsModel model)
        {
            var bookSettings = new BookSettings
            {
                Client = DownloadClient.Readarr
            };

            if (!model.Categories.Any())
            {
                return BadRequest($"At least one category is required.");
            }

            if (model.Categories.Any(x => string.IsNullOrWhiteSpace(x.Name)))
            {
                return BadRequest($"A category name is required.");
            }

            foreach (var category in model.Categories)
            {
                category.Name = category.Name.Trim();
                category.Tags = category.Tags;
            }

            if (new HashSet<string>(model.Categories.Select(x => x.Name.ToLower())).Count != model.Categories.Length)
            {
                return BadRequest($"All categories must have different names.");
            }

            if (new HashSet<int>(model.Categories.Select(x => x.Id)).Count != model.Categories.Length)
            {
                return BadRequest($"All categories must have different ids.");
            }

            if (model.Categories.Any(x => !Regex.IsMatch(x.Name, @"^[\w-]{1,32}$")))
            {
                return BadRequest($"Invalid category names, make sure they only contain alphanumeric characters, dashes and underscores. (No spaces, etc)");
            }

            var readarrSettings = new ReadarrSettingsModel
            {
                Hostname = model.Hostname.Trim(),
                ApiKey = model.ApiKey.Trim(),
                BaseUrl = model.BaseUrl.Trim(),
                Port = model.Port,
                Categories = model.Categories,
                SearchNewRequests = model.SearchNewRequests,
                MonitorNewRequests = model.MonitorNewRequests,
                UseSSL = model.UseSSL,
                Version = model.Version
            };

            DownloadClientsSettingsRepository.SetBook(bookSettings, readarrSettings);

            return Ok(new { ok = true });
        }

        private static ReadarrSettings ConvertToReadarrSettings(TestReadarrSettingsModel model)
        {
            return new ReadarrSettings
            {
                ApiKey = model.ApiKey.Trim(),
                Hostname = model.Hostname.Trim(),
                BaseUrl = model.BaseUrl.Trim(),
                Port = model.Port,
                UseSSL = model.UseSSL,
                Version = model.Version
            };
        }
    }
}
