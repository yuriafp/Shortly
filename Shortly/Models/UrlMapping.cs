using System.ComponentModel.DataAnnotations;

namespace Shortly.API.Models
{
    public class UrlMapping
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(2048)]
        [Url]
        public required string OriginalUrl { get; set; }
        [Required]
        public required string ShortCode { get; set; }
        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpirationDate { get; set; } = DateTimeOffset.UtcNow.AddDays(1);
        public int ClickCount { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;

        public UrlMapping() { }

        public UrlMapping(string originalUrl, string shortCode)
        {
            OriginalUrl = originalUrl;
            ShortCode = shortCode; 
        }
    }
}

