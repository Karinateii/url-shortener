using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces;

/// <summary>
/// Service interface for URL shortening operations
/// </summary>
public interface IUrlService
{
    /// <summary>
    /// Create a new shortened URL
    /// </summary>
    /// <param name="originalUrl">The original URL to shorten</param>
    /// <param name="customCode">Optional custom short code</param>
    /// <param name="title">Optional title for the link</param>
    /// <param name="expiresAt">Optional expiration date</param>
    /// <returns>The created shortened URL</returns>
    Task<ShortenedUrl> CreateShortUrlAsync(string originalUrl, string? customCode = null, string? title = null, DateTime? expiresAt = null);
    
    /// <summary>
    /// Get the original URL for a short code and record the click
    /// </summary>
    /// <param name="shortCode">The short code to look up</param>
    /// <param name="clickInfo">Click event information for analytics</param>
    /// <returns>The original URL or null if not found/expired</returns>
    Task<string?> GetOriginalUrlAsync(string shortCode, ClickEvent? clickInfo = null);
    
    /// <summary>
    /// Get a shortened URL with its analytics
    /// </summary>
    Task<ShortenedUrl?> GetUrlWithAnalyticsAsync(int id);
    
    /// <summary>
    /// Get all shortened URLs
    /// </summary>
    Task<IEnumerable<ShortenedUrl>> GetAllUrlsAsync(int page = 1, int pageSize = 20);
    
    /// <summary>
    /// Delete a shortened URL
    /// </summary>
    Task<bool> DeleteUrlAsync(int id);
    
    /// <summary>
    /// Toggle the active status of a URL
    /// </summary>
    Task<bool> ToggleActiveAsync(int id);
}
