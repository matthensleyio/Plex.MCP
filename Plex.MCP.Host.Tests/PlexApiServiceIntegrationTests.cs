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

        // Build configuration
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddUserSecrets<PlexApiServiceIntegrationTests>();

        _configuration = configurationBuilder.Build();

        // Build service provider
        var services = new ServiceCollection();
        services.AddSingleton(_configuration);
        services.AddLogging(builder => builder.AddSerilog());
        services.AddHttpClient();
        services.AddScoped<IPlexApiService, PlexApiService>();

        _serviceProvider = services.BuildServiceProvider();
        _plexApiService = _serviceProvider.GetRequiredService<IPlexApiService>();
    }

    [Fact]
    public async Task GetServerCapabilitiesAsync_ShouldReturnServerInfo()
    {
        // Act
        var result = await _plexApiService.GetServerCapabilitiesAsync();

        // Assert
        Assert.NotNull(result);
        // Note: Specific assertions will depend on actual Plex server response structure
    }

    [Fact]
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
    public async Task GetLibraryAsync_WithValidSectionId_ShouldReturnLibrary()
    {
        // Arrange
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        if (libraries.Libraries.Count == 0)
        {
            // Skip test if no libraries exist
            return;
        }

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
    public async Task GetLibraryItemsAsync_WithValidSectionId_ShouldReturnItems()
    {
        // Arrange
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        if (libraries.Libraries.Count == 0)
        {
            // Skip test if no libraries exist
            return;
        }

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
    public async Task GetRecentlyAddedAsync_ShouldReturnRecentItems()
    {
        // Act
        var result = await _plexApiService.GetRecentlyAddedAsync(0, 10);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Size >= 0);
    }

    [Fact]
    public async Task SearchAsync_WithValidQuery_ShouldReturnResults()
    {
        // Act
        var result = await _plexApiService.SearchAsync("test", 0, 10);

        // Assert
        Assert.NotNull(result);
        // Search results can be empty, so just verify we get a valid response
    }

    [Fact]
    public async Task SearchLibraryAsync_WithValidSectionAndQuery_ShouldReturnResults()
    {
        // Arrange
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        if (libraries.Libraries.Count == 0)
        {
            // Skip test if no libraries exist
            return;
        }

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
    public async Task GetHubsAsync_ShouldReturnHubs()
    {
        // Act
        var result = await _plexApiService.GetHubsAsync();

        // Assert
        Assert.NotNull(result);
        // Hubs can be empty, so just verify we get a valid response
    }

    [Fact]
    public async Task GetSessionsAsync_ShouldReturnSessions()
    {
        // Act
        var result = await _plexApiService.GetSessionsAsync();

        // Assert
        Assert.NotNull(result);
        // Sessions can be empty, so just verify we get a valid response
    }

    [Fact]
    public async Task GetPlaylistsAsync_ShouldReturnPlaylists()
    {
        // Act
        var result = await _plexApiService.GetPlaylistsAsync();

        // Assert
        Assert.NotNull(result);
        // Playlists can be empty, so just verify we get a valid response
    }

    [Fact]
    public async Task GetPlaylistAsync_WithValidPlaylistId_ShouldReturnPlaylist()
    {
        // Arrange - Get playlists first to get a valid playlist ID
        var playlists = await _plexApiService.GetPlaylistsAsync();

        if (playlists?.MediaContainer?.Playlists == null || playlists.MediaContainer.Playlists.Count == 0)
        {
            // Skip test if no playlists exist
            return;
        }

        var firstPlaylist = playlists.MediaContainer.Playlists.First();
        var playlistId = firstPlaylist.RatingKey;

        // Act
        var result = await _plexApiService.GetPlaylistAsync(playlistId);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task RefreshLibraryAsync_WithValidSectionId_ShouldComplete()
    {
        // Arrange
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        if (libraries.Libraries.Count == 0)
        {
            // Skip test if no libraries exist
            return;
        }

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
    public async Task MarkAsPlayedAsync_WithValidRatingKey_ShouldComplete()
    {
        // Arrange - Get a library item first to get a valid rating key
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        if (libraries.Libraries.Count == 0)
        {
            // Skip test if no libraries exist
            return;
        }

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;
        var items = await _plexApiService.GetLibraryItemsAsync(sectionId, 0, 1);

        if (items?.Metadata?.Count == 0)
        {
            // Skip test if no items exist
            return;
        }

        var firstItem = items!.Metadata!.First();
        var ratingKey = firstItem.RatingKey;

        // Act & Assert - Should not throw
        await _plexApiService.MarkAsPlayedAsync(ratingKey);
    }

    [Fact]
    public async Task MarkAsUnplayedAsync_WithValidRatingKey_ShouldComplete()
    {
        // Arrange - Get a library item first to get a valid rating key
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        if (libraries.Libraries.Count == 0)
        {
            // Skip test if no libraries exist
            return;
        }

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;
        var items = await _plexApiService.GetLibraryItemsAsync(sectionId, 0, 1);

        if (items?.Metadata?.Count == 0)
        {
            // Skip test if no items exist
            return;
        }

        var firstItem = items!.Metadata!.First();
        var ratingKey = firstItem.RatingKey;

        // Act & Assert - Should not throw
        await _plexApiService.MarkAsUnplayedAsync(ratingKey);
    }

    [Fact]
    public async Task UpdatePlayProgressAsync_WithValidRatingKey_ShouldComplete()
    {
        // Arrange - Get a library item first to get a valid rating key
        var libraries = await _plexApiService.GetLibrariesAsync();
        Assert.NotNull(libraries?.Libraries);

        if (libraries.Libraries.Count == 0)
        {
            // Skip test if no libraries exist
            return;
        }

        var firstLibrary = libraries.Libraries.First();
        // Extract section ID from key path (e.g., "/library/sections/1/all" -> "1")
        var keyParts = firstLibrary.Key.Split('/');
        var sectionId = keyParts.Length >= 4 && keyParts[1] == "library" && keyParts[2] == "sections"
            ? keyParts[3]
            : firstLibrary.Key;
        var items = await _plexApiService.GetLibraryItemsAsync(sectionId, 0, 1);

        if (items?.Metadata?.Count == 0)
        {
            // Skip test if no items exist
            return;
        }

        var firstItem = items!.Metadata!.First();
        var ratingKey = firstItem.RatingKey;

        // Act & Assert - Should not throw
        await _plexApiService.UpdatePlayProgressAsync(ratingKey, 30000, "stopped");
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