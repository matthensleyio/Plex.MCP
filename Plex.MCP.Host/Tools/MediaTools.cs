using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Plex.MCP.Host.Models;
using Plex.MCP.Host.Services;
using System.ComponentModel;
using System.Text.Json;

namespace Plex.MCP.Host.Tools;

[McpServerToolType]
public class MediaTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<MediaTools> _logger;

    public MediaTools(IPlexApiService plexApi, ILogger<MediaTools> logger)
    {
        _plexApi = plexApi;
        _logger = logger;
    }

    [McpServerTool, Description("Get metadata for a specific media item")]
    public async Task<string> GetMetadataAsync(
        [Description("Rating key of the media item")] string ratingKey)
    {
        try
        {
            var metadata = await _plexApi.GetMetadataAsync(ratingKey);
            if (metadata == null)
                return $"Media item with rating key '{ratingKey}' not found.";

            return JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata for {RatingKey}", ratingKey);

            // Try to get raw JSON from the API service
            try
            {
                var rawJson = await _plexApi.GetRawJsonAsync($"/library/metadata/{ratingKey}");
                return $"Error retrieving metadata: {ex.Message}\n\nRaw Plex JSON:\n{rawJson}";
            }
            catch
            {
                return $"Error retrieving metadata: {ex.Message}";
            }
        }
    }

    [McpServerTool, Description("Mark a media item as played")]
    public async Task<string> MarkAsPlayedAsync(
        [Description("Rating key of the media item")] string ratingKey)
    {
        try
        {
            await _plexApi.MarkAsPlayedAsync(ratingKey);
            return $"Media item '{ratingKey}' marked as played successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking {RatingKey} as played", ratingKey);
            return $"Error marking as played: {ex.Message}";
        }
    }

    [McpServerTool, Description("Mark a media item as unplayed")]
    public async Task<string> MarkAsUnplayedAsync(
        [Description("Rating key of the media item")] string ratingKey)
    {
        try
        {
            await _plexApi.MarkAsUnplayedAsync(ratingKey);
            return $"Media item '{ratingKey}' marked as unplayed successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking {RatingKey} as unplayed", ratingKey);
            return $"Error marking as unplayed: {ex.Message}";
        }
    }

    [McpServerTool, Description("Update play progress for a media item")]
    public async Task<string> UpdatePlayProgressAsync(
        [Description("Rating key of the media item")] string ratingKey,
        [Description("Current playback time in milliseconds")] int time,
        [Description("Playback state (playing, paused, stopped)")] string state = "stopped")
    {
        try
        {
            var validStates = new[] { "playing", "paused", "stopped" };
            if (!validStates.Contains(state.ToLower()))
                return $"Invalid state '{state}'. Valid states are: {string.Join(", ", validStates)}";

            await _plexApi.UpdatePlayProgressAsync(ratingKey, time, state);
            return $"Play progress updated for media item '{ratingKey}' - Time: {time}ms, State: {state}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating play progress for {RatingKey}", ratingKey);
            return $"Error updating play progress: {ex.Message}";
        }
    }
}