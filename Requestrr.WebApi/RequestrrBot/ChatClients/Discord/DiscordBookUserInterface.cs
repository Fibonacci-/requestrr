using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Requestrr.WebApi.RequestrrBot.Books;
using Requestrr.WebApi.RequestrrBot.Locale;

namespace Requestrr.WebApi.RequestrrBot.ChatClients.Discord
{
    public class DiscordBookUserInterface : IBookUserInterface
    {
        private readonly DiscordInteraction _interactionContext;

        public DiscordBookUserInterface(DiscordInteraction interactionContext)
        {
            _interactionContext = interactionContext;
        }

        public async Task ShowBookSelection(BookRequest request, IReadOnlyList<Book> books)
        {
            // Build option values: prefer internal ReadarrId when available, otherwise encode foreign edition id as f-<id>
            var seen = new HashSet<string>();
            var options = new List<DiscordSelectComponentOption>();

            foreach (var b in books)
            {
                string value = b.ReadarrId != null && b.ReadarrId != "0" ? $"{request.CategoryId}/{b.ReadarrId}" : (string.IsNullOrWhiteSpace(b.ForeignBookId) ? null : $"{request.CategoryId}/f-{b.ForeignBookId}");
                if (string.IsNullOrWhiteSpace(value)) continue;
                if (!seen.Add(value)) continue;
                options.Add(new DiscordSelectComponentOption(LimitStringSize(FormatBookTitle(b)), value));
                if (options.Count >= 15) break;
            }

            try
            {
                System.Console.Error.WriteLine($"ShowBookSelection: building select with {options.Count} options");
                for (int i = 0; i < System.Math.Min(5, options.Count); i++)
                {
                    var o = options[i];
                    System.Console.Error.WriteLine($"  option[{i}]: label='{o.Label}', value='{o.Value}'");
                }
            }
            catch { }

            var select = new DiscordSelectComponent($"BRS/{_interactionContext.User.Id}/{request.CategoryId}", Language.Current.DiscordCommandBookRequestHelpDropdown, options);

            try
            {
                await _interactionContext.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddComponents(select).WithContent(Language.Current.DiscordCommandBookRequestHelp));
            }
            catch (DSharpPlus.Exceptions.BadRequestException bre)
            {
                System.Console.Error.WriteLine("Discord EditOriginalResponseAsync BadRequest: " + bre.ToString());
                // Fallback: try editing without components to avoid invalid component payload
                try
                {
                    await _interactionContext.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(Language.Current.DiscordCommandBookRequestHelp));
                }
                catch (System.Exception ex)
                {
                    System.Console.Error.WriteLine("Discord fallback EditOriginalResponseAsync also failed: " + ex.ToString());
                    throw;
                }
            }
            catch (System.Exception ex)
            {
                System.Console.Error.WriteLine("Discord EditOriginalResponseAsync failed: " + ex.ToString());
                throw;
            }
        }

        public async Task WarnNoBookFoundAsync(string bookName)
        {
            await _interactionContext.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(Language.Current.DiscordCommandBookNotFound.ReplaceTokens(LanguageTokens.BookTitle, bookName)));
        }

        public async Task WarnBookAlreadyRequestedAsync(Book book)
        {
            await _interactionContext.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(Language.Current.DiscordCommandBookAlreadyRequested.ReplaceTokens(book)));
        }

        public async Task WarnBookUnavailableAsync(Book book)
        {
            await _interactionContext.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(Language.Current.DiscordCommandBookUnavailable.ReplaceTokens(book)));
        }

        public async Task DisplayBookDetails(BookRequest request, Book book)
        {
            var requestButton = new DiscordButtonComponent(ButtonStyle.Primary, $"BRC/{_interactionContext.User.Id}/{request.CategoryId}/{book.ReadarrId}", Language.Current.DiscordCommandRequestButton);

            var builder = new DiscordWebhookBuilder()
                .AddEmbed(GenerateBookDetails(book))
                .AddComponents(requestButton)
                .WithContent(Language.Current.DiscordCommandBookRequestConfirm);

            await _interactionContext.EditOriginalResponseAsync(builder);
        }

        private DiscordEmbed GenerateBookDetails(Book book)
        {
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle($"{book.Title} ({book.ReleaseDate?.Split('-')[0] ?? "Unknown"})")
                .WithDescription(book.Overview?.Length > 250 ? book.Overview.Substring(0, 250) + "..." : book.Overview ?? Language.Current.DiscordEmbedBookNoDescription)
                .AddField($"__{Language.Current.DiscordEmbedBookAuthor}__", book.Author, true)
                .WithFooter("Powered by Requestrr");

            if (!string.IsNullOrEmpty(book.CoverUrl))
            {
                embedBuilder.WithImageUrl(book.CoverUrl);
            }

            return embedBuilder.Build();
        }

        private string FormatBookTitle(Book book)
        {
            return $"{book.Title} ({book.ReleaseDate?.Split('-')[0] ?? "Unknown"})";
        }

        private string LimitStringSize(string value, int limit = 100)
        {
            return value.Length > limit ? value.Substring(0, limit - 3) + "..." : value;
        }
    }
}
