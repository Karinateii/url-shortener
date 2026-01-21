using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Web.Pages;

/// <summary>
/// Page model for the dashboard showing all URLs and stats
/// </summary>
public class DashboardModel : PageModel
{
    private readonly IUrlService _urlService;
    
    public DashboardModel(IUrlService urlService)
    {
        _urlService = urlService;
    }
    
    /// <summary>
    /// List of all shortened URLs
    /// </summary>
    public IEnumerable<ShortenedUrl> Urls { get; set; } = Enumerable.Empty<ShortenedUrl>();
    
    /// <summary>
    /// Total number of URLs
    /// </summary>
    public int TotalUrls { get; set; }
    
    /// <summary>
    /// Total clicks across all URLs
    /// </summary>
    public int TotalClicks { get; set; }
    
    /// <summary>
    /// Number of active URLs
    /// </summary>
    public int ActiveUrls { get; set; }
    
    public async Task OnGetAsync()
    {
        Urls = await _urlService.GetAllUrlsAsync(1, 100);
        
        var urlList = Urls.ToList();
        TotalUrls = urlList.Count;
        TotalClicks = urlList.Sum(u => u.ClickCount);
        ActiveUrls = urlList.Count(u => u.IsActive && !u.IsExpired);
    }
    
    /// <summary>
    /// Handle URL deletion
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _urlService.DeleteUrlAsync(id);
        return RedirectToPage();
    }
}
