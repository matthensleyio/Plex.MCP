using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Plex.MCP.Host.Models;
using Plex.MCP.Host.Services;
using System.ComponentModel;
using System.Text.Json;

namespace Plex.MCP.Host.Tools;

[McpServerToolType]
public class SearchTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<SearchTools> _logger;

    public SearchTools(IPlexApiService plexApi, ILogger<SearchTools> logger)
    {
        _plexApi = plexApi;
        _logger = logger;
    }

    [McpServerTool, Description("Search across all libraries for content")]
    public async Task<string> SearchAsync(
        [Description("Search query")] string query,
        [Description("Starting index for pagination (default: 0)")] int start = 0,
        [Description("Number of items to return (default: 50, max: 200)")] int size = 50)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return "Search query cannot be empty.";

            if (size > 200) size = 200;
            if (start < 0) start = 0;

            var results = await _plexApi.SearchAsync(query, start, size);
            if (results == null)
                return $"No results found for '{query}'.";

            return JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for {Query}", query);
            return $"Error performing search: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get discovery hubs from Plex server")]
    public async Task<string> GetHubsAsync()
    {
        try
        {
            var hubs = await _plexApi.GetHubsAsync();
            if (hubs == null)
                return "No hubs found.";

            return JsonSerializer.Serialize(hubs, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hubs");
            return $"Error retrieving hubs: {ex.Message}";
        }
    }
}