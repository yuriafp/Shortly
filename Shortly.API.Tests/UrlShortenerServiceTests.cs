using AutoMapper;
using FluentAssertions;
using Moq;
using Shortly.API.Common;
using Shortly.API.Data.Repositories;
using Shortly.API.Dtos;
using Shortly.API.Models;
using Shortly.API.Services;

namespace Shortly.API.Tests
{
    public class UrlShortenerServiceTests
    {
        private readonly Mock<IUrlShortenerRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UrlShortenerService _service;
        public UrlShortenerServiceTests()
        {
            _repositoryMock = new Mock<IUrlShortenerRepository>();
            _mapperMock = new Mock<IMapper>();

            _service = new UrlShortenerService(_repositoryMock.Object, _mapperMock.Object);
        }

        #region CreateUrlMappingAsync
        [Fact]
        public async Task CreateUrlMappingAsync_WithValidDto_ShouldCreateAndReturnSuccessResult()
        {
            // Arrange
            var inputDto = new UrlMappingCreateDto { OriginalUrl = "https://www.google.com/" };

            var urlEntity = new UrlMapping { OriginalUrl = inputDto.OriginalUrl, ShortCode = string.Empty };

            var expectedResponseDto = new UrlMappingResponseDto { OriginalUrl = inputDto.OriginalUrl, ShortCode = "0001"};

            // Mock
            _mapperMock
                .Setup(m => m.Map<UrlMapping>(inputDto))
                .Returns(urlEntity);

            _repositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask)
                .Callback(() => urlEntity.Id = 1);

            _mapperMock
                .Setup(m => m.Map<UrlMappingResponseDto>(urlEntity))
                .Returns(expectedResponseDto);

            //_mapperMock
            //   .Setup(m => m.Map<UrlMappingResponseDto>(It.Is<UrlMapping>(u => u.Id == 1 && u.ShortCode == "0001")))
            //   .Returns(expectedResponseDto);

            // Act
            var result = await _service.CreateUrlMappingAsync(inputDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ShortCode.Should().NotBeNullOrEmpty();
            result.Value.OriginalUrl.Should().Be(inputDto.OriginalUrl);

            _repositoryMock.Verify(r => r.Add(It.IsAny<UrlMapping>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
        }
        #endregion

        #region GetUrlMappingByShortCodeAsync
        [Fact]
        public async Task GetUrlMappingByShortCodeAsync_ShouldReturnFailure_WhenUrlDoesNotExist()
        {
            // Mock
            _repositoryMock
                .Setup(r => r.GetByShortCodeReadOnlyAsync(It.IsAny<string>()))
                .ReturnsAsync((UrlMapping?)null);

            // Act
            var result = await _service.GetUrlMappingByShortCodeAsync("unknown-code");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(DomainErrors.NotFound);
        }

        [Fact]
        public async Task GetUrlMappingByShortCodeAsync_ShouldReturnFailure_WhenUrlIsExpired()
        {
            // Arrange
            var expiredUrlMapping = new UrlMapping
            {
                Id = 1,
                OriginalUrl = "https://expirad.com",
                ShortCode = "expirad",
                ExpirationDate = DateTimeOffset.UtcNow.AddDays(-1) //setting an expired date.
            };

            // Mock
            _repositoryMock
                .Setup(r => r.GetByShortCodeReadOnlyAsync("expirad"))
                .ReturnsAsync(expiredUrlMapping);

            // Act
            var result = await _service.GetUrlMappingByShortCodeAsync("expirad");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(DomainErrors.Expired);
        }

        [Fact]
        public async Task GetUrlMappingByShortCodeAsync_ShouldReturnSuccess_WhenUrlIsValid()
        {
            // Arrange
            var validUrlMapping = new UrlMapping
            {
                Id = 2,
                OriginalUrl = "https://valid.com",
                ShortCode = "valid",
                ExpirationDate = DateTimeOffset.UtcNow.AddDays(1)
            };

            var expectedDto = new UrlMappingResponseDto { OriginalUrl = "https://valid.com", ShortCode = "valid" };

            // Mock
            _repositoryMock
                .Setup(r => r.GetByShortCodeReadOnlyAsync("valid"))
                .ReturnsAsync(validUrlMapping);

            // AutoMapper mock
            _mapperMock
                .Setup(m => m.Map<UrlMappingResponseDto>(validUrlMapping))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetUrlMappingByShortCodeAsync("valid");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedDto);
        }
        #endregion

        #region DeleteUrlMappingAsync
        [Fact]
        public async Task DeleteUrlMappingAsync_WhenUrlExists_ShouldReturnSuccessAndMarkAsExpired()
        {
            // Arrange
            var existingUrl = new UrlMapping
            {
                Id = 1,
                OriginalUrl = "https://to-delete.com",
                ShortCode = "delete",
                IsDeleted = false
            };

            // Mock
            _repositoryMock
                .Setup(r => r.GetByShortCodeAsync("delete"))
                .ReturnsAsync(existingUrl);

            // Act
            var result = await _service.DeleteUrlMappingAsync("delete");

            // Assert
            result.IsSuccess.Should().BeTrue();

            existingUrl.IsDeleted.Should().BeTrue();

            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUrlMappingAsync_WhenUrlDoesNotExist_ShouldReturnFailure()
        {
            // Mock
            _repositoryMock
                .Setup(r => r.GetByShortCodeAsync(It.IsAny<string>()))
                .ReturnsAsync((UrlMapping?)null);

            // Act
            var result = await _service.DeleteUrlMappingAsync("unknown-code");

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(DomainErrors.NotFound);

            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
        #endregion
    }
}