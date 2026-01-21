namespace UrlShortener.Domain.Interfaces;

/// <summary>
/// Service interface for generating unique short codes
/// </summary>
public interface IShortCodeGenerator
{
    /// <summary>
    /// Generate a unique short code
    /// </summary>
    /// <param name="length">Length of the short code (default 6)</param>
    /// <returns>A unique alphanumeric short code</returns>
    string Generate(int length = 6);
    
    /// <summary>
    /// Validate that a custom short code meets requirements
    /// </summary>
    /// <param name="shortCode">The custom short code to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValid(string shortCode);
}
