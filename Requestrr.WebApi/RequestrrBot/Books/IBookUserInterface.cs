using System.Collections.Generic;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public interface IBookUserInterface
    {
        Task ShowBookSelection(BookRequest request, IReadOnlyList<Book> books);
        Task WarnNoBookFoundAsync(string bookName);
        Task WarnBookAlreadyRequestedAsync(Book book);
        Task WarnBookUnavailableAsync(Book book);
        Task DisplayBookDetails(BookRequest request, Book book);
    }
}
