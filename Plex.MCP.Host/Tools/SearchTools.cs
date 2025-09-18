using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Plex.MCP.Host.Models;
using Plex.MCP.Host.Models.Output;
using Plex.MCP.Host.Models.PlexApi;
using Plex.MCP.Host.Services;
using Plex.MCP.Host.Mcp;
using System.ComponentModel;
using System.Text.Json;

namespace Plex.MCP.Host.Tools;

[McpServerToolType]
public class SearchTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<SearchTools> _logger;
    private readonly McpDispatcher _dispatcher;

    public SearchTools(IPlexApiService plexApi, ILogger<SearchTools> logger, McpDispatcher dispatcher)
    {
        _plexApi = plexApi;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [McpServerTool, Description("Search across all libraries for content")]
    public Task<McpResponse<PlexSearchResponse>> SearchAsync(
        [Description("Search query")] string query,
        [Description("Starting index for pagination (default: 0)")] int start = 0,
        [Description("Number of items to return (default: 50, max: 200)")] int size = 50)
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Search query is required.", nameof(query));
            if (size > 200) size = 200;
            if (start < 0) start = 0;

            var results = await _plexApi.SearchAsync(query, start, size);
            if (results == null)
                throw new InvalidOperationException($"No results found for '{query}'.");

            return results;
        });
    }

    [McpServerTool, Description("Get discovery hubs from Plex server")]
    public Task<McpResponse<PlexHubResponse>> GetHubsAsync()
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            var hubs = await _plexApi.GetHubsAsync();
            if (hubs == null)
                throw new InvalidOperationException("No hubs found.");

            return hubs;
        });
    }
}