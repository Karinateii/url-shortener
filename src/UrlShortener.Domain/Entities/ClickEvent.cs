namespace UrlShortener.Domain.Entities;

/// <summary>
/// Represents a single click/visit event for analytics tracking
/// </summary>
public class ClickEvent
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to the shortened URL
    /// </summary>
    public int ShortenedUrlId { get; set; }
    
    /// <summary>
    /// When the click occurred
    /// </summary>
    public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// IP address of the visitor (anonymized for privacy)
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent string from the browser
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// HTTP referer - where the user came from
    /// </summary>
    public string? Referer { get; set; }
    
    /// <summary>
    /// Country code based on IP geolocation
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Device type: Desktop, Mobile, Tablet
    /// </summary>
    public string? DeviceType { get; set; }
    
    /// <summary>
    /// Browser name: Chrome, Firefox, Safari, etc.
    /// </summary>
    public string? Browser { get; set; }
    
    /// <summary>
    /// Navigation property back to the shortened URL
    /// </summary>
    public virtual ShortenedUrl? ShortenedUrl { get; set; }
}
