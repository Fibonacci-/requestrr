using System;

namespace Requestrr.WebApi.RequestrrBot.Books
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ReadarrId { get; set; } // Internal ID (BookId)
        public string ForeignBookId { get; set; } // Goodreads ID typically
        public string Isbn { get; set; }
        public string CoverUrl { get; set; }
        public string Overview { get; set; }
        public string ReleaseDate { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsMonitored { get; set; }
    }
}
