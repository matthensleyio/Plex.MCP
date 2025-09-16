using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Plex.MCP.Host.Models;
using Plex.MCP.Host.Services;
using System.ComponentModel;
using System.Text.Json;

namespace Plex.MCP.Host.Tools;

[McpServerToolType]
public class PlaylistTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<PlaylistTools> _logger;

    public PlaylistTools(IPlexApiService plexApi, ILogger<PlaylistTools> logger)
    {
        _plexApi = plexApi;
        _logger = logger;
    }

    [McpServerTool, Description("Get all playlists from Plex server")]
    public async Task<string> GetPlaylistsAsync()
    {
        try
        {
            var playlists = await _plexApi.GetPlaylistsAsync();
            if (playlists == null)
                return "No playlists found.";

            return JsonSerializer.Serialize(playlists, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting playlists");
            return $"Error retrieving playlists: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get details and items from a specific playlist")]
    public async Task<string> GetPlaylistAsync(
        [Description("Playlist ID")] string playlistId)
    {
        try
        {
            var playlist = await _plexApi.GetPlaylistAsync(playlistId);
            if (playlist == null)
                return $"Playlist '{playlistId}' not found.";

            return JsonSerializer.Serialize(playlist, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting playlist {PlaylistId}", playlistId);
            return $"Error retrieving playlist: {ex.Message}";
        }
    }
}