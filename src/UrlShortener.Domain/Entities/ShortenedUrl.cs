namespace UrlShortener.Domain.Entities;

/// <summary>
/// Represents a shortened URL with its metadata and settings
/// </summary>
public class ShortenedUrl
{
    public int Id { get; set; }
    
    /// <summary>
    /// The original long URL that will be redirected to
    /// </summary>
    public string OriginalUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// The unique short code used in the shortened URL
    /// Example: "abc123" for https://short.app/abc123
    /// </summary>
    public string ShortCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional custom title for easy identification
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// When this shortened URL was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Optional expiration date - null means never expires
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Total number of times this link has been clicked
    /// </summary>
    public int ClickCount { get; set; } = 0;
    
    /// <summary>
    /// When this link was last accessed
    /// </summary>
    public DateTime? LastClickedAt { get; set; }
    
    /// <summary>
    /// Whether this link is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Navigation property for click analytics
    /// </summary>
    public virtual ICollection<ClickEvent> Clicks { get; set; } = new List<ClickEvent>();
    
    /// <summary>
    /// Check if the URL has expired
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    
    /// <summary>
    /// Check if the URL can be accessed
    /// </summary>
    public bool CanRedirect => IsActive && !IsExpired;
}
