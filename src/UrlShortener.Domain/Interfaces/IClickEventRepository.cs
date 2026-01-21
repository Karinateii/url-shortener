using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces;

/// <summary>
/// Repository interface for click analytics operations
/// </summary>
public interface IClickEventRepository
{
    /// <summary>
    /// Record a new click event
    /// </summary>
    Task AddAsync(ClickEvent clickEvent);
    
    /// <summary>
    /// Get all click events for a specific URL
    /// </summary>
    Task<IEnumerable<ClickEvent>> GetByUrlIdAsync(int urlId);
    
    /// <summary>
    /// Get click events for a URL within a date range
    /// </summary>
    Task<IEnumerable<ClickEvent>> GetByUrlIdAndDateRangeAsync(int urlId, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Get click count grouped by day for a URL
    /// </summary>
    Task<Dictionary<DateTime, int>> GetDailyClicksAsync(int urlId, int days = 30);
    
    /// <summary>
    /// Get top referrers for a URL
    /// </summary>
    Task<Dictionary<string, int>> GetTopReferrersAsync(int urlId, int count = 10);
    
    /// <summary>
    /// Get device type breakdown for a URL
    /// </summary>
    Task<Dictionary<string, int>> GetDeviceBreakdownAsync(int urlId);
    
    /// <summary>
    /// Get browser breakdown for a URL
    /// </summary>
    Task<Dictionary<string, int>> GetBrowserBreakdownAsync(int urlId);
}
