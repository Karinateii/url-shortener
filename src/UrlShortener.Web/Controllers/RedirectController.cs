using Microsoft.AspNetCore.Mvc;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Web.Controllers;

/// <summary>
/// Controller for handling URL redirects
/// </summary>
[ApiController]
public class RedirectController : ControllerBase
{
    private readonly IUrlService _urlService;
    
    public RedirectController(IUrlService urlService)
    {
        _urlService = urlService;
    }
    
    /// <summary>
    /// Redirect to the original URL based on short code
    /// Records click event for analytics
    /// </summary>
    [HttpGet("/{shortCode}")]
    public async Task<IActionResult> RedirectToOriginal(string shortCode)
    {
        // Build click event for analytics
        var clickEvent = new ClickEvent
        {
            ClickedAt = DateTime.UtcNow,
            IpAddress = GetAnonymizedIp(),
            UserAgent = Request.Headers.UserAgent.ToString(),
            Referer = Request.Headers.Referer.ToString(),
            DeviceType = DetectDeviceType(Request.Headers.UserAgent.ToString()),
            Browser = DetectBrowser(Request.Headers.UserAgent.ToString())
        };
        
        // Get original URL and record click
        var originalUrl = await _urlService.GetOriginalUrlAsync(shortCode, clickEvent);
        
        if (originalUrl == null)
        {
            // Redirect to home page with error
            return RedirectToPage("/Index", new { error = "Link not found or expired" });
        }
        
        // Permanent redirect for SEO
        return RedirectPermanent(originalUrl);
    }
    
    /// <summary>
    /// Anonymize IP address for privacy (keep first two octets only)
    /// </summary>
    private string? GetAnonymizedIp()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ip)) return null;
        
        // Anonymize by zeroing last octets
        var parts = ip.Split('.');
        if (parts.Length == 4)
        {
            return $"{parts[0]}.{parts[1]}.0.0";
        }
        
        return ip.Length > 10 ? ip.Substring(0, 10) + "..." : ip;
    }
    
    /// <summary>
    /// Detect device type from user agent
    /// </summary>
    private string DetectDeviceType(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return "Unknown";
        
        userAgent = userAgent.ToLower();
        
        if (userAgent.Contains("mobile") || userAgent.Contains("android") && !userAgent.Contains("tablet"))
            return "Mobile";
        
        if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
            return "Tablet";
        
        return "Desktop";
    }
    
    /// <summary>
    /// Detect browser from user agent
    /// </summary>
    private string DetectBrowser(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return "Unknown";
        
        userAgent = userAgent.ToLower();
        
        if (userAgent.Contains("edg/")) return "Edge";
        if (userAgent.Contains("chrome")) return "Chrome";
        if (userAgent.Contains("firefox")) return "Firefox";
        if (userAgent.Contains("safari") && !userAgent.Contains("chrome")) return "Safari";
        if (userAgent.Contains("opera") || userAgent.Contains("opr")) return "Opera";
        
        return "Other";
    }
}
