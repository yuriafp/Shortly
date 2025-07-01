using System.ComponentModel.DataAnnotations;

namespace Shortly.API.Dtos
{
    public class UrlMappingResponseDto
    {
        public string? OriginalUrl { get; set; }
        public string? ShortCode { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
        public int ClickCount { get; set; }
    }
}
