using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Requestrr.WebApi.RequestrrBot.Books;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Notifications.Books
{
    public class ChannelBookNotifier : IBookNotifier
    {
        private readonly DiscordClient _discordClient;
        private readonly ulong[] _channelIds;
        private readonly ILogger _logger;

        public ChannelBookNotifier(DiscordClient discordClient, ulong[] channelIds, ILogger logger)
        {
            _discordClient = discordClient;
            _channelIds = channelIds;
            _logger = logger;
        }


        public async Task<HashSet<string>> NotifyBookAsync(IReadOnlyCollection<string> userIds, Book book, CancellationToken token)
        {
            HashSet<string> userNotified = new HashSet<string>();

            if (_discordClient.Guilds.Any())
            {
                foreach (ulong channelId in _channelIds)
                {
                    if (token.IsCancellationRequested)
                        return userNotified;

                    try
                    {
                        foreach (DiscordGuild guild in _discordClient.Guilds.Values)
                        {
                            if (guild.Channels.ContainsKey(channelId))
                            {
                                DiscordChannel channel = guild.Channels[channelId];
                                await channel.SendMessageAsync($"The book **{book.Title}** by {book.Author} is now available!");
                                userNotified.UnionWith(userIds);
                                break;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while sending a channel book notification: " + ex.Message);
                    }
                }
            }

            return userNotified;
        }
    }
}
