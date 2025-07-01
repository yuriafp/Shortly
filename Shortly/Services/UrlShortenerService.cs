using AutoMapper;
using Base62;
using Shortly.API.Common;
using Shortly.API.Data.Repositories;
using Shortly.API.Dtos;
using Shortly.API.Models;



namespace Shortly.API.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUrlShortenerRepository _repository;
        private readonly IMapper _mapper;

        public UrlShortenerService(IUrlShortenerRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<Result<UrlMappingResponseDto>> CreateUrlMappingAsync(UrlMappingCreateDto urlMappingCreateDto)
        {
            var urlMappingEntity = _mapper.Map<UrlMapping>(urlMappingCreateDto);

            urlMappingEntity.ShortCode = string.Empty;

            _repository.Add(urlMappingEntity);
            await _repository.SaveChangesAsync();

            urlMappingEntity.ShortCode = EncodingExtensions.ToBase62(urlMappingEntity.Id);

            await _repository.SaveChangesAsync();

            var responseDto = _mapper.Map<UrlMappingResponseDto>(urlMappingEntity);
            return Result<UrlMappingResponseDto>.Success(responseDto);
        }

        public async Task<Result<UrlMappingResponseDto>> GetUrlMappingByShortCodeAsync(string shortCode)
        {
            var urlMappingEntity = await _repository.GetByShortCodeReadOnlyAsync(shortCode);

            if (urlMappingEntity == null)
            {
                return Result<UrlMappingResponseDto>.Failure(DomainErrors.NotFound);
            }

            if (urlMappingEntity.ExpirationDate <= DateTimeOffset.UtcNow)
            {
                return Result<UrlMappingResponseDto>.Failure(DomainErrors.Expired);
            }

            var responseDto = _mapper.Map<UrlMappingResponseDto>(urlMappingEntity);
            return Result<UrlMappingResponseDto>.Success(responseDto);
        }

        public async Task<Result> DeleteUrlMappingAsync(string shortCode)
        {
            var urlMappingEntity = await _repository.GetByShortCodeAsync(shortCode);

            if (urlMappingEntity == null) return Result.Failure(DomainErrors.NotFound);

            urlMappingEntity.IsDeleted = true;
            await _repository.SaveChangesAsync();

            return Result.Success();
        }
        public async Task<Result<string>> ProcessRedirectAndCountAsync(string shortCode)
        {
            var urlMappingEntity = await _repository.GetByShortCodeAsync(shortCode);

            if (urlMappingEntity == null) return Result<string>.Failure(DomainErrors.NotFound);

            if (urlMappingEntity.IsDeleted || urlMappingEntity.ExpirationDate <= DateTimeOffset.UtcNow)
            {
                return Result<string>.Failure(DomainErrors.Expired);
            }

            urlMappingEntity.ClickCount++;
            await _repository.SaveChangesAsync();

            return Result<string>.Success(urlMappingEntity.OriginalUrl);
        }
    }
}