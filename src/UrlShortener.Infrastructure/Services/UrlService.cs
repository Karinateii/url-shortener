using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Services;

/// <summary>
/// URL service implementation with caching for fast redirects
/// </summary>
public class UrlService : IUrlService
{
    private readonly IUrlRepository _urlRepository;
    private readonly IClickEventRepository _clickEventRepository;
    private readonly IShortCodeGenerator _shortCodeGenerator;
    private readonly IMemoryCache _cache;
    
    // Cache settings
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
    private const string CacheKeyPrefix = "url_";
    
    public UrlService(
        IUrlRepository urlRepository,
        IClickEventRepository clickEventRepository,
        IShortCodeGenerator shortCodeGenerator,
        IMemoryCache cache)
    {
        _urlRepository = urlRepository;
        _clickEventRepository = clickEventRepository;
        _shortCodeGenerator = shortCodeGenerator;
        _cache = cache;
    }
    
    /// <summary>
    /// Create a new shortened URL
    /// </summary>
    public async Task<ShortenedUrl> CreateShortUrlAsync(
        string originalUrl, 
        string? customCode = null, 
        string? title = null, 
        DateTime? expiresAt = null)
    {
        // Validate URL format
        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uri) 
            || (uri.Scheme != "http" && uri.Scheme != "https"))
        {
            throw new ArgumentException("Invalid URL format. Must be a valid HTTP or HTTPS URL.");
        }
        
        string shortCode;
        
        // Use custom code or generate a new one
        if (!string.IsNullOrWhiteSpace(customCode))
        {
            // Validate custom code format
            if (!_shortCodeGenerator.IsValid(customCode))
            {
                throw new ArgumentException("Invalid custom code. Must be 3-20 alphanumeric characters.");
            }
            
            // Check if custom code already exists
            if (await _urlRepository.ShortCodeExistsAsync(customCode))
            {
                throw new ArgumentException("This custom code is already taken.");
            }
            
            shortCode = customCode;
        }
        else
        {
            // Generate unique short code
            shortCode = await GenerateUniqueShortCodeAsync();
        }
        
        // Create the shortened URL entity
        var shortenedUrl = new ShortenedUrl
        {
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            Title = title,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
        
        // Save to database
        await _urlRepository.AddAsync(shortenedUrl);
        
        // Add to cache for fast lookups
        _cache.Set(CacheKeyPrefix + shortCode, shortenedUrl.OriginalUrl, _cacheExpiration);
        
        return shortenedUrl;
    }
    
    /// <summary>
    /// Get the original URL for a short code
    /// First checks cache, then database
    /// Records click event for analytics
    /// </summary>
    public async Task<string?> GetOriginalUrlAsync(string shortCode, ClickEvent? clickInfo = null)
    {
        // Try to get from cache first for performance
        if (_cache.TryGetValue(CacheKeyPrefix + shortCode, out string? cachedUrl))
        {
            // Record click asynchronously (fire and forget for speed)
            if (clickInfo != null)
            {
                var url = await _urlRepository.GetByShortCodeAsync(shortCode);
                if (url != null && url.CanRedirect)
                {
                    clickInfo.ShortenedUrlId = url.Id;
                    _ = Task.Run(async () =>
                    {
                        await _clickEventRepository.AddAsync(clickInfo);
                        await _urlRepository.IncrementClickCountAsync(url.Id);
                    });
                }
            }
            return cachedUrl;
        }
        
        // Not in cache, get from database
        var shortenedUrl = await _urlRepository.GetByShortCodeAsync(shortCode);
        
        // Check if URL exists and can be accessed
        if (shortenedUrl == null || !shortenedUrl.CanRedirect)
        {
            return null;
        }
        
        // Add to cache for future requests
        _cache.Set(CacheKeyPrefix + shortCode, shortenedUrl.OriginalUrl, _cacheExpiration);
        
        // Record click event
        if (clickInfo != null)
        {
            clickInfo.ShortenedUrlId = shortenedUrl.Id;
            await _clickEventRepository.AddAsync(clickInfo);
            await _urlRepository.IncrementClickCountAsync(shortenedUrl.Id);
        }
        
        return shortenedUrl.OriginalUrl;
    }
    
    /// <summary>
    /// Get URL with full analytics data
    /// </summary>
    public async Task<ShortenedUrl?> GetUrlWithAnalyticsAsync(int id)
    {
        return await _urlRepository.GetByIdAsync(id);
    }
    
    /// <summary>
    /// Get all URLs with pagination
    /// </summary>
    public async Task<IEnumerable<ShortenedUrl>> GetAllUrlsAsync(int page = 1, int pageSize = 20)
    {
        return await _urlRepository.GetAllAsync(page, pageSize);
    }
    
    /// <summary>
    /// Delete a shortened URL and remove from cache
    /// </summary>
    public async Task<bool> DeleteUrlAsync(int id)
    {
        var url = await _urlRepository.GetByIdAsync(id);
        if (url == null) return false;
        
        // Remove from cache
        _cache.Remove(CacheKeyPrefix + url.ShortCode);
        
        // Delete from database
        await _urlRepository.DeleteAsync(id);
        return true;
    }
    
    /// <summary>
    /// Toggle the active status of a URL
    /// </summary>
    public async Task<bool> ToggleActiveAsync(int id)
    {
        var url = await _urlRepository.GetByIdAsync(id);
        if (url == null) return false;
        
        url.IsActive = !url.IsActive;
        await _urlRepository.UpdateAsync(url);
        
        // Update cache based on new status
        if (url.IsActive)
        {
            _cache.Set(CacheKeyPrefix + url.ShortCode, url.OriginalUrl, _cacheExpiration);
        }
        else
        {
            _cache.Remove(CacheKeyPrefix + url.ShortCode);
        }
        
        return true;
    }
    
    /// <summary>
    /// Generate a unique short code that doesn't exist in database
    /// </summary>
    private async Task<string> GenerateUniqueShortCodeAsync()
    {
        string shortCode;
        int attempts = 0;
        const int maxAttempts = 10;
        
        do
        {
            shortCode = _shortCodeGenerator.Generate();
            attempts++;
            
            if (attempts >= maxAttempts)
            {
                // Increase length if having trouble finding unique codes
                shortCode = _shortCodeGenerator.Generate(8);
            }
        }
        while (await _urlRepository.ShortCodeExistsAsync(shortCode));
        
        return shortCode;
    }
}
