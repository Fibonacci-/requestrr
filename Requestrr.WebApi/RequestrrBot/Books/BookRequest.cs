namespace Requestrr.WebApi.RequestrrBot.Books
{
    public class BookRequest
    {
        public int CategoryId { get; }
        public BookUserRequester User { get; }

        public BookRequest(BookUserRequester user, int categoryId)
        {
            User = user;
            CategoryId = categoryId;
        }
    }

    public class BookUserRequester
    {
        public string UserId { get; set; }
        public string Username { get; set; }

        public BookUserRequester(string userId, string username)
        {
            UserId = userId;
            Username = username;
        }
    }
}
