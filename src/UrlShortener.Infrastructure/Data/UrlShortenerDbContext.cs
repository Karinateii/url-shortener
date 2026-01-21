using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the URL Shortener
/// </summary>
public class UrlShortenerDbContext : DbContext
{
    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
        : base(options)
    {
    }
    
    /// <summary>
    /// Shortened URLs table
    /// </summary>
    public DbSet<ShortenedUrl> ShortenedUrls => Set<ShortenedUrl>();
    
    /// <summary>
    /// Click events table for analytics
    /// </summary>
    public DbSet<ClickEvent> ClickEvents => Set<ClickEvent>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure ShortenedUrl entity
        modelBuilder.Entity<ShortenedUrl>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.Id);
            
            // Short code must be unique and indexed for fast lookups
            entity.HasIndex(e => e.ShortCode)
                  .IsUnique();
            
            // Original URL is required
            entity.Property(e => e.OriginalUrl)
                  .IsRequired()
                  .HasMaxLength(2048);
            
            // Short code configuration
            entity.Property(e => e.ShortCode)
                  .IsRequired()
                  .HasMaxLength(20);
            
            // Title is optional
            entity.Property(e => e.Title)
                  .HasMaxLength(200);
            
            // Index for filtering active URLs
            entity.HasIndex(e => e.IsActive);
            
            // Index for expiration queries
            entity.HasIndex(e => e.ExpiresAt);
        });
        
        // Configure ClickEvent entity
        modelBuilder.Entity<ClickEvent>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.Id);
            
            // Foreign key relationship
            entity.HasOne(e => e.ShortenedUrl)
                  .WithMany(u => u.Clicks)
                  .HasForeignKey(e => e.ShortenedUrlId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Index for querying clicks by URL
            entity.HasIndex(e => e.ShortenedUrlId);
            
            // Index for date range queries
            entity.HasIndex(e => e.ClickedAt);
            
            // IP address is stored anonymized
            entity.Property(e => e.IpAddress)
                  .HasMaxLength(50);
            
            // User agent can be long
            entity.Property(e => e.UserAgent)
                  .HasMaxLength(500);
            
            // Referer URL
            entity.Property(e => e.Referer)
                  .HasMaxLength(2048);
            
            // Country code
            entity.Property(e => e.Country)
                  .HasMaxLength(10);
            
            // Device type
            entity.Property(e => e.DeviceType)
                  .HasMaxLength(50);
            
            // Browser name
            entity.Property(e => e.Browser)
                  .HasMaxLength(100);
        });
    }
}
