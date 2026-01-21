using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for click event operations
/// </summary>
public class ClickEventRepository : IClickEventRepository
{
    private readonly UrlShortenerDbContext _context;
    
    public ClickEventRepository(UrlShortenerDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Record a new click event
    /// </summary>
    public async Task AddAsync(ClickEvent clickEvent)
    {
        _context.ClickEvents.Add(clickEvent);
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Get all click events for a specific URL
    /// </summary>
    public async Task<IEnumerable<ClickEvent>> GetByUrlIdAsync(int urlId)
    {
        return await _context.ClickEvents
            .AsNoTracking()
            .Where(c => c.ShortenedUrlId == urlId)
            .OrderByDescending(c => c.ClickedAt)
            .ToListAsync();
    }
    
    /// <summary>
    /// Get click events within a date range for reporting
    /// </summary>
    public async Task<IEnumerable<ClickEvent>> GetByUrlIdAndDateRangeAsync(int urlId, DateTime startDate, DateTime endDate)
    {
        return await _context.ClickEvents
            .AsNoTracking()
            .Where(c => c.ShortenedUrlId == urlId 
                     && c.ClickedAt >= startDate 
                     && c.ClickedAt <= endDate)
            .OrderByDescending(c => c.ClickedAt)
            .ToListAsync();
    }
    
    /// <summary>
    /// Get click count grouped by day for charting
    /// </summary>
    public async Task<Dictionary<DateTime, int>> GetDailyClicksAsync(int urlId, int days = 30)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);
        
        var clicks = await _context.ClickEvents
            .AsNoTracking()
            .Where(c => c.ShortenedUrlId == urlId && c.ClickedAt >= startDate)
            .GroupBy(c => c.ClickedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();
        
        return clicks.ToDictionary(x => x.Date, x => x.Count);
    }
    
    /// <summary>
    /// Get top referrers for analytics
    /// </summary>
    public async Task<Dictionary<string, int>> GetTopReferrersAsync(int urlId, int count = 10)
    {
        var referrers = await _context.ClickEvents
            .AsNoTracking()
            .Where(c => c.ShortenedUrlId == urlId && !string.IsNullOrEmpty(c.Referer))
            .GroupBy(c => c.Referer)
            .Select(g => new { Referer = g.Key!, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToListAsync();
        
        return referrers.ToDictionary(x => x.Referer, x => x.Count);
    }
    
    /// <summary>
    /// Get device type breakdown for analytics pie chart
    /// </summary>
    public async Task<Dictionary<string, int>> GetDeviceBreakdownAsync(int urlId)
    {
        var devices = await _context.ClickEvents
            .AsNoTracking()
            .Where(c => c.ShortenedUrlId == urlId)
            .GroupBy(c => c.DeviceType ?? "Unknown")
            .Select(g => new { Device = g.Key, Count = g.Count() })
            .ToListAsync();
        
        return devices.ToDictionary(x => x.Device, x => x.Count);
    }
    
    /// <summary>
    /// Get browser breakdown for analytics
    /// </summary>
    public async Task<Dictionary<string, int>> GetBrowserBreakdownAsync(int urlId)
    {
        var browsers = await _context.ClickEvents
            .AsNoTracking()
            .Where(c => c.ShortenedUrlId == urlId)
            .GroupBy(c => c.Browser ?? "Unknown")
            .Select(g => new { Browser = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();
        
        return browsers.ToDictionary(x => x.Browser, x => x.Count);
    }
}
