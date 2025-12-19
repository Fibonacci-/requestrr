using Requestrr.WebApi.RequestrrBot.Notifications.Books;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public class BookNotificationWorkflow : IBookNotificationWorkflow
    {
        private readonly BookNotificationsRepository _notificationsRepository;
        private readonly IBookUserInterface _userInterface;
        private readonly bool _automaticNotificationForNewRequests;


        public BookNotificationWorkflow(
            BookNotificationsRepository bookNotificationsRepository,
            IBookUserInterface userInterface,
            bool automaticNotificationForNewRequests
        )
        {
            _notificationsRepository = bookNotificationsRepository;
            _userInterface = userInterface;
            _automaticNotificationForNewRequests = automaticNotificationForNewRequests;
        }


        public Task NotifyForNewRequestAsync(string userId, Book book)
        {
            if (_automaticNotificationForNewRequests)
            {
                _notificationsRepository.AddNotification(userId, book.ReadarrId);
            }

            return Task.CompletedTask;
        }


        public async Task NotifyForExistingRequestAsync(string userId, Book book)
        {
            if (IsAlreadyNotified(userId, book))
            {
                await _userInterface.WarnBookUnavailableAsync(book);
            }
            else
            {
                await _userInterface.WarnBookUnavailableAsync(book);
            }
        }


        public Task AddNotificationAsync(string userId, string bookId)
        {
            _notificationsRepository.AddNotification(userId, bookId);
            return Task.CompletedTask;
        }


        private bool IsAlreadyNotified(string userId, Book book)
        {
            return _notificationsRepository.HasNotification(userId, book.ReadarrId);
        }
    }
}
