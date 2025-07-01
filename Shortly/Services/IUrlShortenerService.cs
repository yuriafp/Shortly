using Shortly.API.Common;
using Shortly.API.Dtos;

namespace Shortly.API.Services
{
    public interface IUrlShortenerService
    {
        Task<Result<UrlMappingResponseDto>> CreateUrlMappingAsync(UrlMappingCreateDto urlMappingCreateDto);
        Task<Result<UrlMappingResponseDto>> GetUrlMappingByShortCodeAsync(string shortCode);
        Task<Result> DeleteUrlMappingAsync(string shortCode);
        Task<Result<string>> ProcessRedirectAndCountAsync(string shortCode);
    }
}