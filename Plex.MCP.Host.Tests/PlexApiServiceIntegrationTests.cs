using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plex.MCP.Host.Services;
using Serilog;
using System.IO;
using System.Net.Http;

namespace Plex.MCP.Host.Tests;

public class PlexApiServiceIntegrationTests : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPlexApiService _plexApiService;
    private readonly IConfiguration _configuration;

    public PlexApiServiceIntegrationTests()
    {
        // Configure Serilog for tests
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        // Build configuration - prioritize user secrets for sensitive data
        var configurationBuilder = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<PlexApiServiceIntegrationTests>();

        _configuration = configurationBuilder.Build();

        // Validate that we have the required configuration for integration tests
        ValidateConfiguration();

        // Build service provider
        var services = new ServiceCollection();
        services.AddSingleton(_configuration);
        services.AddLogging(builder => builder.AddSerilog());
        services.AddHttpClient();
        services.AddScoped<IPlexApiService, PlexApiService>();

        _serviceProvider = services.BuildServiceProvider();
        _plexApiService = _serviceProvider.GetRequiredService<IPlexApiService>();
    }

    private void ValidateConfiguration()
    {
        var serverUrl = _configuration["Plex:ServerUrl"];
        var token = _configuration["Plex:Token"];

        if (string.IsNullOrEmpty(serverUrl))
        {
            throw new InvalidOperationException(
                "Plex:ServerUrl configuration is missing. Set user secrets with: " +
                "dotnet user-secrets set \"Plex:ServerUrl\" \"http://your-server:32400\"");
        }

        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException(
                "Plex:Token configuration is missing or using placeholder value. Set user secrets with: " +
                "dotnet user-secrets set \"Plex:Token\" \"your-actual-token\"");
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetServerCapabilitiesAsync_ShouldReturnServerInfo()
    {
        // Act
        var result = await _plexApiService.GetServerCapabilitiesAsync();

        // Assert
        Assert.NotNull(result);
        // Note: Specific assertions will depend on actual Plex server response structure
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetLibrariesAsync_ShouldReturnLibraries()
    {
        // Act
        var result = await _plexApiService.GetLibrariesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Libraries);
        Assert.True(result.Libraries.Count >= 0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetLibraryAsync_WithValidSectionId_ShouldReturnLibrary()
    {
        // Arrange
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        Assert.True(libraries.Libraries.Count > 0,
            "No libraries found on Plex server. Please ensure your Plex server has at least one library configured.");

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;

        // Act
        var result = await _plexApiService.GetLibraryAsync(sectionId);

        // Assert
        Assert.NotNull(result);
        // The returned library should exist, but the Key format may differ
        Assert.NotNull(result.Title);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetLibraryItemsAsync_WithValidSectionId_ShouldReturnItems()
    {
        // Arrange
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        Assert.True(libraries.Libraries.Count > 0,
            "No libraries found on Plex server. Please ensure your Plex server has at least one library configured.");

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;

        // Act
        var result = await _plexApiService.GetLibraryItemsAsync(sectionId, 0, 10);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Size >= 0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetRecentlyAddedAsync_ShouldReturnRecentItems()
    {
        // Act
        var result = await _plexApiService.GetRecentlyAddedAsync(0, 10);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Size >= 0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SearchAsync_WithValidQuery_ShouldReturnResults()
    {
        // Act
        var result = await _plexApiService.SearchAsync("test", 0, 10);

        // Assert
        Assert.NotNull(result);
        // Search results can be empty, so just verify we get a valid response
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SearchLibraryAsync_WithValidSectionAndQuery_ShouldReturnResults()
    {
        // Arrange
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        Assert.True(libraries.Libraries.Count > 0,
            "No libraries found on Plex server. Please ensure your Plex server has at least one library configured.");

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;

        // Act
        var result = await _plexApiService.SearchLibraryAsync(sectionId, "test", 0, 10);

        // Assert
        Assert.NotNull(result);
        // Search results can be empty, so just verify we get a valid response
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetHubsAsync_ShouldReturnHubs()
    {
        // Act
        var result = await _plexApiService.GetHubsAsync();

        // Assert
        Assert.NotNull(result);
        // Hubs can be empty, so just verify we get a valid response
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetSessionsAsync_ShouldReturnSessions()
    {
        // Act
        var result = await _plexApiService.GetSessionsAsync();

        // Assert
        Assert.NotNull(result);
        // Sessions can be empty, so just verify we get a valid response
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetPlaylistsAsync_ShouldReturnPlaylists()
    {
        // Act
        var result = await _plexApiService.GetPlaylistsAsync();

        // Assert
        Assert.NotNull(result);
        // Playlists can be empty, so just verify we get a valid response
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetPlaylistAsync_WithValidPlaylistId_ShouldReturnPlaylist()
    {
        // Arrange - Get playlists first to get a valid playlist ID
        var playlists = await _plexApiService.GetPlaylistsAsync();

        Assert.True(playlists?.MediaContainer?.Playlists != null && playlists.MediaContainer.Playlists.Count > 0,
            "No playlists found on Plex server. Please create at least one playlist to run this test.");

        var firstPlaylist = playlists.MediaContainer.Playlists.First();
        var playlistId = firstPlaylist.RatingKey;

        // Act
        var result = await _plexApiService.GetPlaylistAsync(playlistId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task RefreshLibraryAsync_WithValidSectionId_ShouldComplete()
    {
        // Arrange
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        Assert.True(libraries.Libraries.Count > 0,
            "No libraries found on Plex server. Please ensure your Plex server has at least one library configured.");

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;

        // Act & Assert - Should not throw
        await _plexApiService.RefreshLibraryAsync(sectionId);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task MarkAsPlayedAsync_WithValidRatingKey_ShouldComplete()
    {
        // Arrange - Get a library item first to get a valid rating key
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        Assert.True(libraries.Libraries.Count > 0,
            "No libraries found on Plex server. Please ensure your Plex server has at least one library configured.");

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;
        var items = await _plexApiService.GetLibraryItemsAsync(sectionId, 0, 1);

        Assert.True(items?.Metadata?.Count > 0,
            $"No media items found in library section '{sectionId}'. Please ensure the library contains at least one media item.");

        var firstItem = items!.Metadata!.First();
        var ratingKey = firstItem.RatingKey;

        // Store original state for cleanup
        var originalMetadata = await _plexApiService.GetMetadataAsync(ratingKey);
        var wasPlayed = originalMetadata?.ViewCount > 0;

        try
        {
            // Act & Assert - Should not throw
            await _plexApiService.MarkAsPlayedAsync(ratingKey);
        }
        finally
        {
            // Cleanup - Restore original played state
            if (!wasPlayed)
            {
                await _plexApiService.MarkAsUnplayedAsync(ratingKey);
            }
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task MarkAsUnplayedAsync_WithValidRatingKey_ShouldComplete()
    {
        // Arrange - Get a library item first to get a valid rating key
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        Assert.True(libraries.Libraries.Count > 0,
            "No libraries found on Plex server. Please ensure your Plex server has at least one library configured.");

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;
        var items = await _plexApiService.GetLibraryItemsAsync(sectionId, 0, 1);

        Assert.True(items?.Metadata?.Count > 0,
            $"No media items found in library section '{sectionId}'. Please ensure the library contains at least one media item.");

        var firstItem = items!.Metadata!.First();
        var ratingKey = firstItem.RatingKey;

        // Store original state for cleanup
        var originalMetadata = await _plexApiService.GetMetadataAsync(ratingKey);
        var wasPlayed = originalMetadata?.ViewCount > 0;

        try
        {
            // Act & Assert - Should not throw
            await _plexApiService.MarkAsUnplayedAsync(ratingKey);
        }
        finally
        {
            // Cleanup - Restore original played state
            if (wasPlayed)
            {
                await _plexApiService.MarkAsPlayedAsync(ratingKey);
            }
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdatePlayProgressAsync_WithValidRatingKey_ShouldComplete()
    {
        // Arrange - Get a library item first to get a valid rating key
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        Assert.True(libraries.Libraries.Count > 0,
            "No libraries found on Plex server. Please ensure your Plex server has at least one library configured.");

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;
        var items = await _plexApiService.GetLibraryItemsAsync(sectionId, 0, 1);

        Assert.True(items?.Metadata?.Count > 0,
            $"No media items found in library section '{sectionId}'. Please ensure the library contains at least one media item.");

        var firstItem = items!.Metadata!.First();
        var ratingKey = firstItem.RatingKey;

        try
        {
            // Act & Assert - Should not throw
            await _plexApiService.UpdatePlayProgressAsync(ratingKey, 30000, "stopped");
        }
        finally
        {
            // Cleanup - Reset play progress to 0
            await _plexApiService.UpdatePlayProgressAsync(ratingKey, 0, "stopped");
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateMetadataAsync_WithValidRatingKey_ShouldComplete()
    {
        // Arrange - Get a library item first to get a valid rating key
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        Assert.True(libraries.Libraries.Count > 0,
            "No libraries found on Plex server. Please ensure your Plex server has at least one library configured.");

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;
        var items = await _plexApiService.GetLibraryItemsAsync(sectionId, 0, 1);

        Assert.True(items?.Metadata?.Count > 0,
            $"No media items found in library section '{sectionId}'. Please ensure the library contains at least one media item.");

        var firstItem = items!.Metadata!.First();
        var ratingKey = firstItem.RatingKey;

        // Store original metadata for cleanup
        var originalMetadata = await _plexApiService.GetMetadataAsync(ratingKey);
        Assert.NotNull(originalMetadata);

        var uniqueTestIdentifier = Guid.NewGuid().ToString();
        // Test metadata to update
        var metadataFields = new Dictionary<string, object>
        {
            ["title"] = $"{originalMetadata.Title} {uniqueTestIdentifier}",
            ["summary"] = "Updated by integration test on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        try
        {
            // Act & Assert - Should not throw
            await _plexApiService.UpdateMetadataAsync(ratingKey, metadataFields);

            // Verify the update by getting the metadata again
            var updatedMetadata = await _plexApiService.GetMetadataAsync(ratingKey);
            Assert.NotNull(updatedMetadata);
            Assert.Contains(uniqueTestIdentifier, updatedMetadata.Title);
        }
        finally
        {
            // Cleanup - Restore original metadata
            var cleanupFields = new Dictionary<string, object>
            {
                ["title"] = originalMetadata.Title,
                ["summary"] = originalMetadata.Summary ?? ""
            };
            await _plexApiService.UpdateMetadataAsync(ratingKey, cleanupFields);
        }
    }

    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposableProvider)
        {
            disposableProvider.Dispose();
        }
        Log.CloseAndFlush();
    }
}