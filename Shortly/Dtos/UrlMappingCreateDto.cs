using System.ComponentModel.DataAnnotations;

namespace Shortly.API.Dtos
{
    public class UrlMappingCreateDto
    {
        [Required]
        [StringLength(2048)]
        [Url]
        public required string OriginalUrl { get; set; }
    }
}
