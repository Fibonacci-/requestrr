using System.Linq;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public class BookRequestingWorkflow
    {
        private readonly BookUserRequester _user;
        private readonly int _categoryId;
        private readonly IBookSearcher _searcher;
        private readonly IBookRequester _requester;
        private readonly IBookUserInterface _userInterface;
        private readonly IBookNotificationWorkflow _notificationWorkflow;

        public BookRequestingWorkflow(
            BookUserRequester user,
            int categoryId,
            IBookSearcher searcher,
            IBookRequester requester,
            IBookUserInterface userInterface,
            IBookNotificationWorkflow notificationWorkflow)
        {
            _user = user;
            _categoryId = categoryId;
            _searcher = searcher;
            _requester = requester;
            _userInterface = userInterface;
            _notificationWorkflow = notificationWorkflow;
        }

        public async Task SearchBookAsync(string bookName)
        {
            var searchedBooks = await _searcher.SearchBookAsync(new BookRequest(_user, _categoryId), bookName);

            if (searchedBooks.Any())
            {
                if (searchedBooks.Count > 1)
                {
                    await _userInterface.ShowBookSelection(new BookRequest(_user, _categoryId), searchedBooks);
                }
                else if (searchedBooks.Count == 1)
                {
                    var selection = searchedBooks.Single();
                    await HandleBookSelectionAsync(selection.ReadarrId);
                }
            }
            else
            {
                await _userInterface.WarnNoBookFoundAsync(bookName);
            }
        }

        public async Task HandleBookSelectionAsync(string bookId)
        {
            var book = await _searcher.GetBookDetailsAsync(new BookRequest(_user, _categoryId), bookId);

            if (book == null)
            {
                await _userInterface.WarnNoBookFoundAsync(bookId);
                return;
            }

            if (book.IsMonitored)
            {
                await _userInterface.WarnBookAlreadyRequestedAsync(book);
            }
            else
            {
                await _userInterface.DisplayBookDetails(new BookRequest(_user, _categoryId), book);
            }
        }

        public async Task RequestBookAsync(string bookId)
        {
            var book = await _searcher.GetBookDetailsAsync(new BookRequest(_user, _categoryId), bookId);
            await _requester.RequestBookAsync(new BookRequest(_user, _categoryId), bookId);
            await _notificationWorkflow.NotifyForNewRequestAsync(_user.UserId, book);
        }
    }
}
