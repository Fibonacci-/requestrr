using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Requestrr.WebApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Requestrr.WebApi.RequestrrBot.DownloadClients.Readarr.ReadarrClient;

namespace Requestrr.WebApi.RequestrrBot.DownloadClients.Readarr
{
    public class ReadarrClientV1
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReadarrClient> _logger;
        private ReadarrSettingsProvider _readarrSettingProvider;
        private ReadarrSettings _readarrSettings => _readarrSettingProvider.Provide();

        private string BaseURL => GetBaseURL(_readarrSettings);


        public ReadarrClientV1(IHttpClientFactory httpClientFactory, ILogger<ReadarrClient> logger, ReadarrSettingsProvider readarrSettingsProvider)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _readarrSettingProvider = readarrSettingsProvider;
        }

        public static async Task TestConnectionAsync(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.BaseUrl) && !settings.BaseUrl.StartsWith("/"))
            {
                throw new Exception("Invalid base URL, must start with /");
            }

            var testSuccessful = false;

            try
            {
                var response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/config/host");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Invalid api key");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Incorrect api version");
                }

                try
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JObject.Parse(responseString);

                    if (!jsonResponse.urlBase.ToString().Equals(settings.BaseUrl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception("Base url does not match what is set in Readarr");
                    }
                }
                catch
                {
                    throw new Exception("Base url does not match what is set in Readarr");
                }

                testSuccessful = true;
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Error while testing Readarr connection: " + ex.Message);
                throw new Exception("Invalid host and/or port");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error while testing Readarr connection: " + ex.Message);

                if (ex.GetType() == typeof(Exception))
                {
                    throw;
                }
                else
                {
                    throw new Exception("Invalid host and/or port");
                }
            }

            if (!testSuccessful)
            {
                throw new Exception("Invalid host and/or port");
            }
        }



        public static async Task<IList<JSONRootPath>> GetRootPaths(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            try
            {
                HttpResponseMessage response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/rootfolder");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<JSONRootPath>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error while getting Readarr root paths: " + ex.Message);
            }

            throw new Exception("An error occurred while getting Readarr root paths");
        }


        public static async Task<IList<JSONProfile>> GetProfiles(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            try
            {
                HttpResponseMessage response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/qualityprofile");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<JSONProfile>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error while getting Readarr profiles: " + ex.Message);
            }

            throw new Exception("An error occurred while getting Readarr profiles");
        }

        public static async Task<IList<JSONProfile>> GetMetadataProfiles(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            try
            {
                HttpResponseMessage response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/metadataprofile");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<JSONProfile>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error while getting Readarr metadata profiles: " + ex.Message);
            }

            throw new Exception("An error occurred while getting Readarr metadata profiles");
        }

        public static async Task<IList<JSONTag>> GetTags(HttpClient httpClient, ILogger<ReadarrClient> logger, ReadarrSettings settings)
        {
            try
            {
                HttpResponseMessage response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/tag");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<JSONTag>>(jsonResponse);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error while getting Readarr tags: " + ex.Message);
            }

            throw new Exception("An error occurred while getting Readarr tags");
        }


        public async Task<IList<JSONBook>> SearchBookAsync(string searchTerm)
        {
            string url = null;
            try
            {
                string term = Uri.EscapeDataString(searchTerm.Trim());
                url = $"{BaseURL}/book/lookup?term={term}";
                HttpResponseMessage response = await HttpGetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    string resp = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Readarr book lookup failed. URL: {Url}, StatusCode: {StatusCode}, Response: {Response}", url, (int)response.StatusCode, resp);
                    throw new Exception($"ReadarrBookLookup failed: {response.ReasonPhrase} - {resp}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<JSONBook>>(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while searching for book with Readarr (url: {url}): {ex.Message}");
                throw new Exception("An error occurred while searching for book with Readarr");
            }
        }

        public async Task<JSONBook> GetBookAsync(string bookId)
        {
            try
            {
                // Support lookup token for foreign edition id: f-<foreignEditionId>
                if (!string.IsNullOrWhiteSpace(bookId) && (bookId.StartsWith("f-") || bookId.StartsWith("f:")))
                {
                    var foreignId = bookId.Substring(2);
                    string term = Uri.EscapeDataString(foreignId);
                    HttpResponseMessage response = await HttpGetAsync($"{BaseURL}/book/lookup?term={term}");
                    await response.ThrowIfNotSuccessfulAsync("ReadarrBookLookup failed", x => x.error);

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<JSONBook>>(jsonResponse);

                    var match = list.FirstOrDefault(x => (!string.IsNullOrWhiteSpace(x.ForeignEditionId) && x.ForeignEditionId == foreignId)
                                                         || (!string.IsNullOrWhiteSpace(x.ForeignBookId) && x.ForeignBookId == foreignId)
                                                         || (!string.IsNullOrWhiteSpace(x.TitleSlug) && x.TitleSlug == foreignId));

                    return match;
                }

                // If it's a numeric ID, we can get it directly
                if (int.TryParse(bookId, out int id))
                {
                    HttpResponseMessage response = await HttpGetAsync($"{BaseURL}/book/{id}");
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var book = JsonConvert.DeserializeObject<JSONBook>(jsonResponse);
                    return book;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting book details for {bookId} with Readarr: " + ex.Message);
                throw new Exception("An error occurred while getting book details with Readarr");
            }
        }


        public async Task<Dictionary<string, Books.Book>> SearchAvailableBooksAsync(HashSet<string> bookIds, CancellationToken token)
        {
            try
            {
                List<Books.Book> convertedBooks = new List<Books.Book>();

                foreach (string bookId in bookIds)
                {
                    JSONBook existingBook = await GetBookAsync(bookId);
                    if (existingBook != null && existingBook.Id > 0 && existingBook.Monitored)
                    {
                        // Convert to Book using the parent ReadarrClient Convert method
                        var book = new Books.Book
                        {
                            Title = existingBook.Title,
                            Author = existingBook.Author?.AuthorName ?? existingBook.AuthorTitle,
                            ReadarrId = existingBook.Id.ToString(),
                            ForeignBookId = existingBook.ForeignBookId,
                            Isbn = existingBook.Isbn,
                            CoverUrl = existingBook.Images?.FirstOrDefault(x => x.CoverType == "cover")?.RemoteUrl
                                       ?? existingBook.Images?.FirstOrDefault(x => x.CoverType == "cover")?.Url,
                            Overview = existingBook.Overview,
                            ReleaseDate = existingBook.ReleaseDate?.ToString("yyyy-MM-dd"),
                            IsAvailable = existingBook.Id > 0 && existingBook.Monitored,
                            IsMonitored = existingBook.Monitored
                        };
                        convertedBooks.Add(book);
                    }
                }

                return convertedBooks.Where(x => x.IsAvailable).ToDictionary(x => x.ReadarrId, x => x);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching available books with Readarr: " + ex.Message);
            }

            throw new Exception("An error occurred while searching available books with Readarr");
        }


        public async Task<JSONBook> CreateBookType1Async(JSONBook book, int qualityProfileId, int metadataProfileId, string rootFolderPath, int[] tags, bool monitored, bool searchForMissing)
        {
            try
            {
                book.QualityProfileId = qualityProfileId;
                book.MetadataProfileId = metadataProfileId;
                book.Monitored = monitored;
                book.RootFolderPath = rootFolderPath;
                book.Tags = tags.ToList();

                book.AddOptions = new JSONAddOptions
                {
                    SearchForNewBook = searchForMissing
                };

                var content = JsonConvert.SerializeObject(book);
                var response = await HttpPostAsync($"{BaseURL}/book", content);

                await response.ThrowIfNotSuccessfulAsync("ReadarrBookCreation failed", x => x.error);

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<JSONBook>(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating book {book.Title} in Readarr: " + ex.Message);
                throw;
            }
        }


        private Task<HttpResponseMessage> HttpGetAsync(string url)
        {
            return HttpGetAsync(_httpClientFactory.CreateClient(), _readarrSettings, url);
        }

        private static async Task<HttpResponseMessage> HttpGetAsync(HttpClient client, ReadarrSettings settings, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("X-Api-Key", settings.ApiKey);

            using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5)))
            {
                return await client.SendAsync(request, cts.Token);
            }
        }

        private async Task<HttpResponseMessage> HttpPostAsync(string url, string content)
        {
            StringContent postRequest = new StringContent(content, Encoding.UTF8, "application/json");
            postRequest.Headers.Clear();
            postRequest.Headers.Add("Content-Type", "application/json");
            postRequest.Headers.Add("X-Api-Key", _readarrSettings.ApiKey);

            HttpClient client = _httpClientFactory.CreateClient();
            return await client.PostAsync(url, postRequest);
        }

        private static string GetBaseURL(ReadarrSettings settings)
        {
            var protocol = settings.UseSSL ? "https" : "http";
            return $"{protocol}://{settings.Hostname}:{settings.Port}{settings.BaseUrl}/api/v{settings.Version}";
        }
    }
}
