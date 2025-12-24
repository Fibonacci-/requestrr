using Microsoft.Extensions.Logging;
using Requestrr.WebApi.RequestrrBot.Books;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Readarr
{
    public class ReadarrClient : IBookSearcher, IBookRequester
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReadarrClient> _logger;
        private readonly ReadarrSettingsProvider _settingsProvider;

        public ReadarrClient(IHttpClientFactory httpClientFactory, ILogger<ReadarrClient> logger, ReadarrSettingsProvider settingsProvider)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _settingsProvider = settingsProvider;
        }

        public static Task TestConnectionAsync(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            return ReadarrClientV1.TestConnectionAsync(httpClient, logger, settings);
        }

        public static Task<IList<ReadarrClient.JSONRootPath>> GetRootPaths(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            return ReadarrClientV1.GetRootPaths(httpClient, logger, settings);
        }

        public static Task<IList<ReadarrClient.JSONProfile>> GetProfiles(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            return ReadarrClientV1.GetProfiles(httpClient, logger, settings);
        }

        public static Task<IList<ReadarrClient.JSONProfile>> GetMetadataProfiles(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            return ReadarrClientV1.GetMetadataProfiles(httpClient, logger, settings);
        }

        public static Task<IList<ReadarrClient.JSONTag>> GetTags(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            return ReadarrClientV1.GetTags(httpClient, logger, settings);
        }

        public async Task<IReadOnlyList<Book>> SearchBookAsync(BookRequest request, string bookName)
        {
            var client = CreateInstance<ReadarrClientV1>();
            var books = await client.SearchBookAsync(bookName);
            return books.Select(Convert).ToList();
        }

        public async Task<Book> GetBookDetailsAsync(BookRequest request, string readarrBookId)
        {
            var client = CreateInstance<ReadarrClientV1>();
            var book = await client.GetBookAsync(readarrBookId);
            return book != null ? Convert(book) : null;
        }

        public async Task RequestBookAsync(BookRequest request, string readarrBookId)
        {
            var client = CreateInstance<ReadarrClientV1>();
            var book = await client.GetBookAsync(readarrBookId);

            if (book != null)
            {
                var settings = _settingsProvider.Provide();
                var category = settings.Categories.Single(x => x.Id == request.CategoryId);

                await client.CreateBookType1Async(
                    book,
                    category.ProfileId,
                    category.MetadataProfileId,
                    category.RootFolder,
                    category.Tags,
                    settings.MonitorNewRequests,
                    settings.SearchNewRequests);
            }
        }

        public async Task<Dictionary<string, Book>> SearchAvailableBooksAsync(HashSet<string> bookIds, CancellationToken token)
        {
            var client = CreateInstance<ReadarrClientV1>();
            return await client.SearchAvailableBooksAsync(bookIds, token);
        }


        private T CreateInstance<T>() where T : class
        {
            return new ReadarrClientV1(_httpClientFactory, _logger, _settingsProvider) as T;
        }


        private Book Convert(ReadarrClient.JSONBook jsonBook)
        {
            return new Book
            {
                Title = jsonBook.Title,
                Author = jsonBook.Author?.AuthorName ?? jsonBook.AuthorTitle,
                ReadarrId = jsonBook.Id.ToString(),
                ForeignBookId = !string.IsNullOrWhiteSpace(jsonBook.ForeignEditionId) ? jsonBook.ForeignEditionId : jsonBook.ForeignBookId, // Prefer edition id if present
                Isbn = jsonBook.Isbn,
                CoverUrl = jsonBook.Images?.FirstOrDefault(x => x.CoverType == "cover")?.RemoteUrl
                           ?? jsonBook.Images?.FirstOrDefault(x => x.CoverType == "cover")?.Url,
                Overview = jsonBook.Overview,
                ReleaseDate = jsonBook.ReleaseDate?.ToString("yyyy-MM-dd"),
                IsAvailable = jsonBook.Id > 0 && jsonBook.Monitored, // Simplified check
                IsMonitored = jsonBook.Monitored
            };
        }

        public class JSONBook
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string AuthorTitle { get; set; }
            public string TitleSlug { get; set; }
            public string Overview { get; set; }
            public string Isbn { get; set; }
            public string ForeignBookId { get; set; }
            public string ForeignEditionId { get; set; }
            public bool Monitored { get; set; }
            public int AuthorId { get; set; }
            public JSONAuthor Author { get; set; }
            public List<JSONImage> Images { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public JSONRatings Ratings { get; set; }
            public int QualityProfileId { get; set; }
            public int MetadataProfileId { get; set; }
            public string RootFolderPath { get; set; }
            public JSONAddOptions AddOptions { get; set; }
            public List<int> Tags { get; set; }
            public DateTime Added { get; set; }
        }

        public class JSONAuthor
        {
            public int Id { get; set; }
            public string AuthorName { get; set; }
            public List<JSONImage> Images { get; set; }
        }

        public class JSONImage
        {
            public string CoverType { get; set; }
            public string Url { get; set; }
            public string RemoteUrl { get; set; }
        }

        public class JSONRatings
        {
            public decimal Value { get; set; }
            public int Votes { get; set; }
        }

        public class JSONAddOptions
        {
            public bool SearchForNewBook { get; set; }
        }

        public class JSONRootPath
        {
            public string path { get; set; }
            public bool accessible { get; set; }
            public long freeSpace { get; set; }
            public int id { get; set; }
        }

        public class JSONProfile
        {
            public string name { get; set; }
            public int id { get; set; }
        }

        public class JSONTag
        {
            public string label { get; set; }
            public int id { get; set; }
        }
    }
}
