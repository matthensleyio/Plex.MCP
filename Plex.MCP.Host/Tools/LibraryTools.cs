using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Plex.MCP.Host.Models;
using Plex.MCP.Host.Services;
using System.ComponentModel;
using System.Text.Json;

namespace Plex.MCP.Host.Tools;

[McpServerToolType]
public class LibraryTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<LibraryTools> _logger;

    public LibraryTools(IPlexApiService plexApi, ILogger<LibraryTools> logger)
    {
        _plexApi = plexApi;
        _logger = logger;
    }

    [McpServerTool, Description("Get all library sections from Plex server")]
    public async Task<string> GetLibrariesAsync()
    {
        try
        {
            var libraries = await _plexApi.GetLibrariesAsync();
            if (libraries == null)
                return "No libraries found.";

            return JsonSerializer.Serialize(libraries, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting libraries");
            return $"Error retrieving libraries: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get details about a specific library section")]
    public async Task<string> GetLibraryAsync(
        [Description("Library section ID")] string sectionId)
    {
        try
        {
            var library = await _plexApi.GetLibraryAsync(sectionId);
            if (library == null)
                return $"Library section '{sectionId}' not found.";

            return JsonSerializer.Serialize(library, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting library {SectionId}", sectionId);
            return $"Error retrieving library: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get items from a specific library section")]
    public async Task<string> GetLibraryItemsAsync(
        [Description("Library section ID")] string sectionId,
        [Description("Starting index for pagination (default: 0)")] int start = 0,
        [Description("Number of items to return (default: 50, max: 200)")] int size = 50)
    {
        try
        {
            if (size > 200) size = 200;
            if (start < 0) start = 0;

            var items = await _plexApi.GetLibraryItemsAsync(sectionId, start, size);
            if (items == null)
                return $"No items found in library section '{sectionId}'.";

            return JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting library items for section {SectionId}", sectionId);
            return $"Error retrieving library items: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get recently added content across all libraries")]
    public async Task<string> GetRecentlyAddedAsync(
        [Description("Starting index for pagination (default: 0)")] int start = 0,
        [Description("Number of items to return (default: 50, max: 200)")] int size = 50)
    {
        try
        {
            if (size > 200) size = 200;
            if (start < 0) start = 0;

            var items = await _plexApi.GetRecentlyAddedAsync(start, size);
            if (items == null)
                return "No recently added content found.";

            return JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recently added content");
            return $"Error retrieving recently added content: {ex.Message}";
        }
    }

    [McpServerTool, Description("Refresh metadata for a specific library section")]
    public async Task<string> RefreshLibraryAsync(
        [Description("Library section ID")] string sectionId)
    {
        try
        {
            await _plexApi.RefreshLibraryAsync(sectionId);
            return $"Library section '{sectionId}' refresh initiated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing library {SectionId}", sectionId);
            return $"Error refreshing library: {ex.Message}";
        }
    }

    [McpServerTool, Description("Search within a specific library section")]
    public async Task<string> SearchLibraryAsync(
        [Description("Library section ID")] string sectionId,
        [Description("Search query")] string query,
        [Description("Starting index for pagination (default: 0)")] int start = 0,
        [Description("Number of items to return (default: 50, max: 200)")] int size = 50)
    {
        try
        {
            if (size > 200) size = 200;
            if (start < 0) start = 0;

            var results = await _plexApi.SearchLibraryAsync(sectionId, query, start, size);
            if (results == null)
                return $"No results found for '{query}' in library section '{sectionId}'.";

            return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching library {SectionId} for {Query}", sectionId, query);
            return $"Error searching library: {ex.Message}";
        }
    }
}