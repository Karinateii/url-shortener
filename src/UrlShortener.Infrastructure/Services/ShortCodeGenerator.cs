using System.Security.Cryptography;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Services;

/// <summary>
/// Service for generating unique short codes for URLs
/// Uses cryptographically secure random generation
/// </summary>
public class ShortCodeGenerator : IShortCodeGenerator
{
    // Characters used for generating short codes
    // Excludes similar-looking characters (0, O, l, 1, I) for readability
    private const string Characters = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ23456789";
    
    // Reserved words that cannot be used as custom slugs
    private static readonly HashSet<string> ReservedWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "api", "admin", "dashboard", "login", "logout", "register",
        "settings", "profile", "help", "about", "contact", "privacy",
        "terms", "health", "status", "create", "new", "edit", "delete"
    };
    
    /// <summary>
    /// Generate a unique short code using cryptographically secure random bytes
    /// </summary>
    /// <param name="length">Length of the short code (default 6)</param>
    /// <returns>A random alphanumeric short code</returns>
    public string Generate(int length = 6)
    {
        // Ensure minimum length of 4 for uniqueness
        if (length < 4) length = 4;
        
        // Use cryptographically secure random number generator
        var bytes = RandomNumberGenerator.GetBytes(length);
        var result = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            // Map each byte to a character from our alphabet
            result[i] = Characters[bytes[i] % Characters.Length];
        }
        
        return new string(result);
    }
    
    /// <summary>
    /// Validate that a custom short code meets requirements:
    /// - Length between 3 and 20 characters
    /// - Only alphanumeric characters and hyphens
    /// - Not a reserved word
    /// - Doesn't start or end with hyphen
    /// </summary>
    public bool IsValid(string shortCode)
    {
        // Check length
        if (string.IsNullOrWhiteSpace(shortCode) || shortCode.Length < 3 || shortCode.Length > 20)
            return false;
        
        // Check for reserved words
        if (ReservedWords.Contains(shortCode))
            return false;
        
        // Check for valid characters (alphanumeric and hyphens)
        foreach (char c in shortCode)
        {
            if (!char.IsLetterOrDigit(c) && c != '-')
                return false;
        }
        
        // Can't start or end with hyphen
        if (shortCode.StartsWith('-') || shortCode.EndsWith('-'))
            return false;
        
        // No consecutive hyphens
        if (shortCode.Contains("--"))
            return false;
        
        return true;
    }
}
