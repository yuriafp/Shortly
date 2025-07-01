using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using Shortly.API.Common;
using Shortly.API.Dtos;
using Shortly.API.Models;
using Shortly.API.Services;
using System.Text.Json;

namespace Shortly.API.Controllers
{
    [Route("api/urls")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlShortenerService _urlShortenerService;

        public UrlsController(IUrlShortenerService urlShortenerService) => _urlShortenerService = urlShortenerService ?? throw new ArgumentNullException(nameof(urlShortenerService));

        [HttpPost]
        [ProducesResponseType(typeof(UrlMappingResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] UrlMappingCreateDto requestDto)
        {
            var result = await _urlShortenerService.CreateUrlMappingAsync(requestDto);

            if (result.IsFailure)
            {
                return result.Error switch
                {
                    _ => BadRequest(new { Error = "An unexpected error occurred." })
                };
            }

            var responseDto = result.Value;

            return CreatedAtAction(nameof(GetByShortCode), new { shortCode = responseDto.ShortCode }, responseDto);
        }

        [HttpGet("{shortCode}")]
        [ProducesResponseType(typeof(UrlMappingResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByShortCode([FromRoute] string shortCode)
        {
            var result = await _urlShortenerService.GetUrlMappingByShortCodeAsync(shortCode);

            if (result.IsFailure)
            {
                return result.Error switch
                {
                    DomainErrors.NotFound => NotFound(),
                    DomainErrors.Expired => StatusCode(StatusCodes.Status410Gone, "This URL has expired."),
                    _ => Problem("An unexpected error occurred.")
                };
            }

            return Ok(result.Value);
        }

        [HttpDelete("{shortCode}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Remove([FromRoute] string shortCode)
        {
            var result = await _urlShortenerService.DeleteUrlMappingAsync(shortCode);

            if (result.IsFailure) return NotFound();

            return NoContent();
        }
    }
}


