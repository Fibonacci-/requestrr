using System.ComponentModel.DataAnnotations;

namespace Requestrr.WebApi.Controllers.DownloadClients.Readarr
{
    public class ReadarrPath
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Path { get; set; }
    }
}
