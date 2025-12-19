using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public interface IBookSearcher
    {
        Task<IReadOnlyList<Book>> SearchBookAsync(BookRequest request, string bookName);
        Task<Book> GetBookDetailsAsync(BookRequest request, string readarrBookId);
        Task<Dictionary<string, Book>> SearchAvailableBooksAsync(HashSet<string> bookIds, CancellationToken token);
    }
}
