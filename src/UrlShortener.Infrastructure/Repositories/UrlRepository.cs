using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for URL operations using Entity Framework Core
/// </summary>
public class UrlRepository : IUrlRepository
{
    private readonly UrlShortenerDbContext _context;
    
    public UrlRepository(UrlShortenerDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Get a shortened URL by its short code
    /// Uses AsNoTracking for read-only performance
    /// </summary>
    public async Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode)
    {
        return await _context.ShortenedUrls
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    }
    
    /// <summary>
    /// Get a shortened URL by its ID with click events loaded
    /// </summary>
    public async Task<ShortenedUrl?> GetByIdAsync(int id)
    {
        return await _context.ShortenedUrls
            .Include(u => u.Clicks)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    /// <summary>
    /// Get all shortened URLs with pagination
    /// Ordered by creation date descending (newest first)
    /// </summary>
    public async Task<IEnumerable<ShortenedUrl>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        return await _context.ShortenedUrls
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    /// <summary>
    /// Get total count of URLs for pagination
    /// </summary>
    public async Task<int> GetCountAsync()
    {
        return await _context.ShortenedUrls.CountAsync();
    }
    
    /// <summary>
    /// Check if a short code already exists in the database
    /// </summary>
    public async Task<bool> ShortCodeExistsAsync(string shortCode)
    {
        return await _context.ShortenedUrls
            .AnyAsync(u => u.ShortCode == shortCode);
    }
    
    /// <summary>
    /// Add a new shortened URL to the database
    /// </summary>
    public async Task<ShortenedUrl> AddAsync(ShortenedUrl url)
    {
        _context.ShortenedUrls.Add(url);
        await _context.SaveChangesAsync();
        return url;
    }
    
    /// <summary>
    /// Update an existing shortened URL
    /// </summary>
    public async Task UpdateAsync(ShortenedUrl url)
    {
        _context.ShortenedUrls.Update(url);
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Delete a shortened URL by ID
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var url = await _context.ShortenedUrls.FindAsync(id);
        if (url != null)
        {
            _context.ShortenedUrls.Remove(url);
            await _context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Increment click count and update last clicked timestamp
    /// Uses raw SQL for performance to avoid loading the entire entity
    /// </summary>
    public async Task IncrementClickCountAsync(int id)
    {
        await _context.ShortenedUrls
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.ClickCount, u => u.ClickCount + 1)
                .SetProperty(u => u.LastClickedAt, DateTime.UtcNow));
    }
}
