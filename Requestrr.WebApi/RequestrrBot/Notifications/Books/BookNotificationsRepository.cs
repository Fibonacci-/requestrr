using Requestrr.WebApi.RequestrrBot.Movies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Requestrr.WebApi.RequestrrBot.Notifications.Books
{
    public class BookNotificationsRepository
    {
        private class Notification
        {
            public string UserId { get; }
            public string BookId { get; }
            public Notification(string userId, string bookId)
            {
                UserId = userId;
                BookId = bookId;
            }

            public override bool Equals(object obj)
            {
                return obj is Notification notification &&
                    UserId == notification.UserId &&
                    BookId == notification.BookId;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(UserId, BookId);
            }
        }


        private HashSet<Notification> _notifications = new HashSet<Notification>();
        private object _lock = new object();

        public BookNotificationsRepository()
        {
            var notifications = NotificationsFile.Read();
            var books = notifications?.Books;

            if (books == null)
            {
                NotificationsFile.WriteBook(new System.Collections.Generic.Dictionary<string, string[]>());
                return;
            }

            foreach (var notification in books)
            {
                foreach (var bookId in notification.BookId)
                {
                    _notifications.Add(new Notification(notification.UserId.ToString(), (string)bookId));
                }
            }
        }



        public void AddNotification(string userId, string bookId)
        {
            lock (_lock)
            {
                if (_notifications.Add(new Notification(userId, bookId)))
                {
                    NotificationsFile.WriteBook(_notifications.GroupBy(x => x.UserId).ToDictionary(x => x.Key, x => x.Select(y => y.BookId).ToArray()));
                }
            }
        }



        public void RemoveNotification(string userId, string bookId)
        {
            lock (_lock)
            {
                if (_notifications.Remove(new Notification(userId, bookId)))
                {
                    NotificationsFile.WriteBook(_notifications.GroupBy(x => x.UserId).ToDictionary(x => x.Key, x => x.Select(y => y.BookId).ToArray()));
                }
            }
        }


        public Dictionary<string, HashSet<string>> GetAllBookNotifications()
        {
            lock (_lock)
            {
                return _notifications
                    .GroupBy(x => x.BookId)
                    .ToDictionary(x => x.Key, x => new HashSet<string>(x.Select(y => y.UserId)));
            }
        }


        public bool HasNotification(string userId, string bookId)
        {
            var hasRequest = false;

            lock (_lock)
            {
                hasRequest = _notifications.Contains(new Notification(userId, bookId));
            }

            return hasRequest;
        }
    }
}
