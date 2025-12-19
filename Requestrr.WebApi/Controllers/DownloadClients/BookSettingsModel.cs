using Requestrr.WebApi.Controllers.DownloadClients.Readarr;
using System.ComponentModel.DataAnnotations;

namespace Requestrr.WebApi.Controllers.DownloadClients
{
    public class BookSettingsModel
    {
        [Required]
        public string Client { get; set; }

        [Required]
        public ReadarrSettingsModel Readarr { get; set; }

        [Required]
        public string[] OtherCategories { get; set; }
    }
}
