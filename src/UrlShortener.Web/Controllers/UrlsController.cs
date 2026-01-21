using Microsoft.AspNetCore.Mvc;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Web.Controllers;

/// <summary>
/// API controller for URL shortening operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UrlsController : ControllerBase
{
    private readonly IUrlService _urlService;
    private readonly IClickEventRepository _clickEventRepository;
    
    public UrlsController(IUrlService urlService, IClickEventRepository clickEventRepository)
    {
        _urlService = urlService;
        _clickEventRepository = clickEventRepository;
    }
    
    /// <summary>
    /// Create a new shortened URL
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUrlRequest request)
    {
        try
        {
            var result = await _urlService.CreateShortUrlAsync(
                request.OriginalUrl,
                request.CustomCode,
                request.Title,
                request.ExpiresAt);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                result.Id,
                result.ShortCode,
                result.OriginalUrl,
                result.Title,
                result.CreatedAt,
                result.ExpiresAt,
                ShortUrl = $"{Request.Scheme}://{Request.Host}/{result.ShortCode}"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get all shortened URLs with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var urls = await _urlService.GetAllUrlsAsync(page, pageSize);
        
        return Ok(urls.Select(u => new
        {
            u.Id,
            u.ShortCode,
            u.OriginalUrl,
            u.Title,
            u.ClickCount,
            u.CreatedAt,
            u.ExpiresAt,
            u.IsActive,
            u.LastClickedAt,
            ShortUrl = $"{Request.Scheme}://{Request.Host}/{u.ShortCode}"
        }));
    }
    
    /// <summary>
    /// Get a specific URL with analytics
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var url = await _urlService.GetUrlWithAnalyticsAsync(id);
        
        if (url == null)
            return NotFound(new { error = "URL not found" });
        
        // Get analytics data
        var dailyClicks = await _clickEventRepository.GetDailyClicksAsync(id, 30);
        var topReferrers = await _clickEventRepository.GetTopReferrersAsync(id);
        var deviceBreakdown = await _clickEventRepository.GetDeviceBreakdownAsync(id);
        var browserBreakdown = await _clickEventRepository.GetBrowserBreakdownAsync(id);
        
        return Ok(new
        {
            url.Id,
            url.ShortCode,
            url.OriginalUrl,
            url.Title,
            url.ClickCount,
            url.CreatedAt,
            url.ExpiresAt,
            url.IsActive,
            url.LastClickedAt,
            ShortUrl = $"{Request.Scheme}://{Request.Host}/{url.ShortCode}",
            Analytics = new
            {
                DailyClicks = dailyClicks,
                TopReferrers = topReferrers,
                DeviceBreakdown = deviceBreakdown,
                BrowserBreakdown = browserBreakdown
            }
        });
    }
    
    /// <summary>
    /// Delete a shortened URL
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _urlService.DeleteUrlAsync(id);
        
        if (!success)
            return NotFound(new { error = "URL not found" });
        
        return NoContent();
    }
    
    /// <summary>
    /// Toggle active status of a URL
    /// </summary>
    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var success = await _urlService.ToggleActiveAsync(id);
        
        if (!success)
            return NotFound(new { error = "URL not found" });
        
        return Ok(new { message = "Status toggled successfully" });
    }
}

/// <summary>
/// Request model for creating a new shortened URL
/// </summary>
public class CreateUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;
    public string? CustomCode { get; set; }
    public string? Title { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
