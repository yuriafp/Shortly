using Shortly.API.Models;

namespace Shortly.API.Data.Repositories
{
    public interface IReadUrlRepository
    {
        Task<UrlMapping?> GetByShortCodeAsync(string shortCode);
        Task<UrlMapping?> GetByShortCodeReadOnlyAsync(string shortCode);
    }
    public interface IWriteUrlRepository
    {
        void Add(UrlMapping urlMapping);
        Task SaveChangesAsync();
    }
    public interface IUrlShortenerRepository : IReadUrlRepository, IWriteUrlRepository
    {
    }
}
