using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Web.Pages;

/// <summary>
/// Page model for the home page with URL shortening form
/// </summary>
public class IndexModel : PageModel
{
    private readonly IUrlService _urlService;
    
    public IndexModel(IUrlService urlService)
    {
        _urlService = urlService;
    }
    
    /// <summary>
    /// Original URL to shorten
    /// </summary>
    [BindProperty]
    public string OriginalUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional custom short code
    /// </summary>
    [BindProperty]
    public string? CustomCode { get; set; }
    
    /// <summary>
    /// Optional title for the link
    /// </summary>
    [BindProperty]
    public string? Title { get; set; }
    
    /// <summary>
    /// Error message to display
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Success message to display
    /// </summary>
    public string? SuccessMessage { get; set; }
    
    /// <summary>
    /// The created shortened URL result
    /// </summary>
    public CreatedUrlResult? CreatedUrl { get; set; }
    
    public void OnGet()
    {
        // Check for error parameter from redirect
        if (Request.Query.ContainsKey("error"))
        {
            ErrorMessage = Request.Query["error"];
        }
    }
    
    /// <summary>
    /// Handle form submission to create a new short URL
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(OriginalUrl))
        {
            ErrorMessage = "Please enter a URL to shorten";
            return Page();
        }
        
        try
        {
            var result = await _urlService.CreateShortUrlAsync(
                OriginalUrl,
                CustomCode,
                Title);
            
            CreatedUrl = new CreatedUrlResult
            {
                ShortUrl = $"{Request.Scheme}://{Request.Host}/{result.ShortCode}",
                OriginalUrl = result.OriginalUrl,
                ShortCode = result.ShortCode
            };
            
            SuccessMessage = "URL shortened successfully!";
            
            // Clear the form
            OriginalUrl = string.Empty;
            CustomCode = null;
            Title = null;
        }
        catch (ArgumentException ex)
        {
            ErrorMessage = ex.Message;
        }
        
        return Page();
    }
}

/// <summary>
/// Result model for displaying the created URL
/// </summary>
public class CreatedUrlResult
{
    public string ShortUrl { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
}
