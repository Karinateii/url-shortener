using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces;

/// <summary>
/// Repository interface for URL operations
/// </summary>
public interface IUrlRepository
{
    /// <summary>
    /// Get a shortened URL by its short code
    /// </summary>
    Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode);
    
    /// <summary>
    /// Get a shortened URL by its ID
    /// </summary>
    Task<ShortenedUrl?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get all shortened URLs with pagination
    /// </summary>
    Task<IEnumerable<ShortenedUrl>> GetAllAsync(int page = 1, int pageSize = 20);
    
    /// <summary>
    /// Get total count of URLs
    /// </summary>
    Task<int> GetCountAsync();
    
    /// <summary>
    /// Check if a short code already exists
    /// </summary>
    Task<bool> ShortCodeExistsAsync(string shortCode);
    
    /// <summary>
    /// Add a new shortened URL
    /// </summary>
    Task<ShortenedUrl> AddAsync(ShortenedUrl url);
    
    /// <summary>
    /// Update an existing shortened URL
    /// </summary>
    Task UpdateAsync(ShortenedUrl url);
    
    /// <summary>
    /// Delete a shortened URL
    /// </summary>
    Task DeleteAsync(int id);
    
    /// <summary>
    /// Increment click count and update last clicked time
    /// </summary>
    Task IncrementClickCountAsync(int id);
}
