using System.ComponentModel.DataAnnotations;

namespace AzureLinkboard.Api.Model
{
    public class SaveUrlRequest
    {
        [Required]
        public string Url { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        public string Tags { get; set; }
    }
}
