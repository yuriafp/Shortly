using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Shortly.API.Models;

namespace Shortly.API.Data.Repositories
{
    public class UrlShortenerRepository : IUrlShortenerRepository
    {
        private readonly AppDbContext _appDbContext;

        public UrlShortenerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void Add(UrlMapping urlMapping)
        {
            ArgumentNullException.ThrowIfNull(urlMapping, nameof(urlMapping));
            _appDbContext.UrlMappings.Add(urlMapping);
        }
        public async Task<UrlMapping?> GetByShortCodeAsync(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode)) throw new ArgumentNullException(nameof(shortCode));

            return await _appDbContext.UrlMappings.SingleOrDefaultAsync(x => x.ShortCode == shortCode);
        }

        public async Task<UrlMapping?> GetByShortCodeReadOnlyAsync(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode)) throw new ArgumentNullException(nameof(shortCode));

            return await _appDbContext.UrlMappings.AsNoTracking().SingleOrDefaultAsync(x => x.ShortCode == shortCode);
        }

        public async Task SaveChangesAsync() => await _appDbContext.SaveChangesAsync();

    }
}
