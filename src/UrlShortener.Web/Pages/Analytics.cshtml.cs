using Microsoft.AspNetCore.Mvc.RazorPages;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Web.Pages;

/// <summary>
/// Page model for viewing detailed analytics for a specific URL
/// </summary>
public class AnalyticsModel : PageModel
{
    private readonly IUrlService _urlService;
    private readonly IClickEventRepository _clickEventRepository;
    
    public AnalyticsModel(IUrlService urlService, IClickEventRepository clickEventRepository)
    {
        _urlService = urlService;
        _clickEventRepository = clickEventRepository;
    }
    
    /// <summary>
    /// The shortened URL with analytics
    /// </summary>
    public ShortenedUrl? Url { get; set; }
    
    /// <summary>
    /// Daily click counts for chart
    /// </summary>
    public Dictionary<DateTime, int> DailyClicks { get; set; } = new();
    
    /// <summary>
    /// Top referrer sources
    /// </summary>
    public Dictionary<string, int> TopReferrers { get; set; } = new();
    
    /// <summary>
    /// Device type breakdown
    /// </summary>
    public Dictionary<string, int> DeviceBreakdown { get; set; } = new();
    
    /// <summary>
    /// Browser breakdown
    /// </summary>
    public Dictionary<string, int> BrowserBreakdown { get; set; } = new();
    
    /// <summary>
    /// List of last 30 days for chart
    /// </summary>
    public List<DateTime> Last30Days { get; set; } = new();
    
    /// <summary>
    /// Maximum daily clicks for chart scaling
    /// </summary>
    public int MaxDailyClicks { get; set; }
    
    public async Task OnGetAsync(int id)
    {
        Url = await _urlService.GetUrlWithAnalyticsAsync(id);
        
        if (Url != null)
        {
            // Get analytics data
            DailyClicks = await _clickEventRepository.GetDailyClicksAsync(id, 30);
            TopReferrers = await _clickEventRepository.GetTopReferrersAsync(id, 5);
            DeviceBreakdown = await _clickEventRepository.GetDeviceBreakdownAsync(id);
            BrowserBreakdown = await _clickEventRepository.GetBrowserBreakdownAsync(id);
            
            // Build list of last 30 days
            for (int i = 29; i >= 0; i--)
            {
                Last30Days.Add(DateTime.UtcNow.Date.AddDays(-i));
            }
            
            // Calculate max for chart scaling
            MaxDailyClicks = DailyClicks.Values.DefaultIfEmpty(0).Max();
            if (MaxDailyClicks == 0) MaxDailyClicks = 1;
        }
    }
}
