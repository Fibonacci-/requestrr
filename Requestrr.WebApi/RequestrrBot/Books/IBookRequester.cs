using System.Threading.Tasks;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public interface IBookRequester
    {
        Task RequestBookAsync(BookRequest request, string readarrBookId);
    }
}
