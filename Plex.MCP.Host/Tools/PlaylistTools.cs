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
public class PlaylistTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<PlaylistTools> _logger;
    private readonly McpDispatcher _dispatcher;

    public PlaylistTools(IPlexApiService plexApi, ILogger<PlaylistTools> logger, McpDispatcher dispatcher)
    {
        _plexApi = plexApi;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [McpServerTool, Description("Get all playlists from Plex server")]
    public Task<McpResponse<PlexPlaylistResponse>> GetPlaylistsAsync()
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            var playlists = await _plexApi.GetPlaylistsAsync();
            if (playlists == null)
                throw new InvalidOperationException("No playlists found.");

            return playlists;
        });
    }

    [McpServerTool, Description("Get details and items from a specific playlist")]
    public Task<McpResponse<PlexPlaylistResponse>> GetPlaylistAsync(
        [Description("Playlist ID")] string playlistId)
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(playlistId))
                throw new ArgumentException("Playlist ID is required.", nameof(playlistId));

            var playlist = await _plexApi.GetPlaylistAsync(playlistId);
            if (playlist == null)
                throw new InvalidOperationException($"Playlist '{playlistId}' not found.");

            return playlist;
        });
    }
}