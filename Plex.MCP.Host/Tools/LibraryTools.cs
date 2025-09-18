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
public class LibraryTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<LibraryTools> _logger;
    private readonly McpDispatcher _dispatcher;

    public LibraryTools(IPlexApiService plexApi, ILogger<LibraryTools> logger, McpDispatcher dispatcher)
    {
        _plexApi = plexApi;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [McpServerTool, Description("Get all library sections from Plex server")]
    public Task<McpResponse<PlexLibrariesResponse>> GetLibrariesAsync()
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            var libraries = await _plexApi.GetLibrariesAsync();
            if (libraries == null)
                throw new InvalidOperationException("No libraries found.");

            return libraries;
        });
    }

    [McpServerTool, Description("Get details about a specific library section")]
    public Task<McpResponse<PlexLibrary>> GetLibraryAsync(
        [Description("Library section ID")] string sectionId)
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(sectionId))
                throw new ArgumentException("Section ID is required.", nameof(sectionId));

            var library = await _plexApi.GetLibraryAsync(sectionId);
            if (library == null)
                throw new InvalidOperationException($"Library section '{sectionId}' not found.");

            return library;
        });
    }

    [McpServerTool, Description("Get items from a specific library section")]
    public Task<McpResponse<PlexMediaContainer>> GetLibraryItemsAsync(
        [Description("Library section ID")] string sectionId,
        [Description("Starting index for pagination (default: 0)")] int start = 0,
        [Description("Number of items to return (default: 50, max: 200)")] int size = 50)
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(sectionId))
                throw new ArgumentException("Section ID is required.", nameof(sectionId));
            if (size > 200) size = 200;
            if (start < 0) start = 0;

            var items = await _plexApi.GetLibraryItemsAsync(sectionId, start, size);
            if (items == null)
                throw new InvalidOperationException($"No items found in library section '{sectionId}'.");

            return items;
        });
    }

    [McpServerTool, Description("Get recently added content across all libraries")]
    public Task<McpResponse<PlexMediaContainer>> GetRecentlyAddedAsync(
        [Description("Starting index for pagination (default: 0)")] int start = 0,
        [Description("Number of items to return (default: 50, max: 200)")] int size = 50)
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            if (size > 200) size = 200;
            if (start < 0) start = 0;

            var items = await _plexApi.GetRecentlyAddedAsync(start, size);
            if (items == null)
                throw new InvalidOperationException("No recently added content found.");

            return items;
        });
    }

    [McpServerTool, Description("Refresh metadata for a specific library section")]
    public Task<McpResponse<string>> RefreshLibraryAsync(
        [Description("Library section ID")] string sectionId)
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(sectionId))
                throw new ArgumentException("Section ID is required.", nameof(sectionId));

            await _plexApi.RefreshLibraryAsync(sectionId);
            return $"Library section '{sectionId}' refresh initiated successfully.";
        });
    }

    [McpServerTool, Description("Search within a specific library section")]
    public Task<McpResponse<PlexMediaContainer>> SearchLibraryAsync(
        [Description("Library section ID")] string sectionId,
        [Description("Search query")] string query,
        [Description("Starting index for pagination (default: 0)")] int start = 0,
        [Description("Number of items to return (default: 50, max: 200)")] int size = 50)
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(sectionId))
                throw new ArgumentException("Section ID is required.", nameof(sectionId));
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Search query is required.", nameof(query));
            if (size > 200) size = 200;
            if (start < 0) start = 0;

            var results = await _plexApi.SearchLibraryAsync(sectionId, query, start, size);
            if (results == null)
                throw new InvalidOperationException($"No results found for '{query}' in library section '{sectionId}'.");

            return results;
        });
    }
}