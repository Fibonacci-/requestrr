using System;
using System.ComponentModel.DataAnnotations;

namespace Requestrr.WebApi.Controllers.DownloadClients.Readarr
{
    public class ReadarrSettingsModel : TestReadarrSettingsModel
    {
        [Required]
        public ReadarrSettingsCategory[] Categories { get; set; } = Array.Empty<ReadarrSettingsCategory>();

        public bool SearchNewRequests { get; set; }
        public bool MonitorNewRequests { get; set; }
    }

    public class ReadarrSettingsCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProfileId { get; set; }
        public int MetadataProfileId { get; set; }
        public string RootFolder { get; set; }
        public int[] Tags { get; set; } = Array.Empty<int>();
    }
}
