using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public class DisabledBookNotificationWorkflow : IBookNotificationWorkflow
    {
        public Task NotifyForNewRequestAsync(string userId, Book book)
        {
            return Task.CompletedTask;
        }

        public Task NotifyForExistingRequestAsync(string userId, Book book)
        {
            return Task.CompletedTask;
        }

        public Task AddNotificationAsync(string userId, string bookId)
        {
            return Task.CompletedTask;
        }
    }
}
