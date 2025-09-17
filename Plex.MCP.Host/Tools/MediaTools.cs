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

    [McpServerTool, Description("Update metadata for a media item (movie, show, episode, audiobook, etc.)")]
    public async Task<string> UpdateMediaMetadataAsync(
        [Description("Rating key of the media item")] string ratingKey,
        [Description("Title of the media item")] string? title = null,
        [Description("Summary/description of the media item")] string? summary = null,
        [Description("User rating (0.0 to 10.0)")] float? rating = null,
        [Description("Content rating (G, PG, PG-13, R, etc.)")] string? contentRating = null,
        [Description("Studio/publisher name")] string? studio = null,
        [Description("Release year")] int? year = null,
        [Description("Lock title field to prevent overwriting")] bool lockTitle = false,
        [Description("Lock summary field to prevent overwriting")] bool lockSummary = false,
        [Description("Lock rating field to prevent overwriting")] bool lockRating = false,
        [Description("Lock content rating field to prevent overwriting")] bool lockContentRating = false,
        [Description("Lock studio field to prevent overwriting")] bool lockStudio = false,
        [Description("Lock year field to prevent overwriting")] bool lockYear = false)
    {
        try
        {
            if (title == null && summary == null && rating == null && contentRating == null && studio == null && year == null)
                return "No metadata fields provided to update. Please specify at least one field.";

            if (rating.HasValue && (rating.Value < 0 || rating.Value > 10))
                return "Rating must be between 0.0 and 10.0";

            var metadataFields = new Dictionary<string, object>();

            if (title != null)
            {
                metadataFields["title"] = lockTitle
                    ? new Dictionary<string, object> { ["value"] = title, ["locked"] = true }
                    : title;
            }

            if (summary != null)
            {
                metadataFields["summary"] = lockSummary
                    ? new Dictionary<string, object> { ["value"] = summary, ["locked"] = true }
                    : summary;
            }

            if (rating.HasValue)
            {
                metadataFields["userRating"] = lockRating
                    ? new Dictionary<string, object> { ["value"] = rating.Value.ToString("F1"), ["locked"] = true }
                    : rating.Value.ToString("F1");
            }

            if (contentRating != null)
            {
                metadataFields["contentRating"] = lockContentRating
                    ? new Dictionary<string, object> { ["value"] = contentRating, ["locked"] = true }
                    : contentRating;
            }

            if (studio != null)
            {
                metadataFields["studio"] = lockStudio
                    ? new Dictionary<string, object> { ["value"] = studio, ["locked"] = true }
                    : studio;
            }

            if (year.HasValue)
            {
                metadataFields["year"] = lockYear
                    ? new Dictionary<string, object> { ["value"] = year.Value.ToString(), ["locked"] = true }
                    : year.Value.ToString();
            }

            await _plexApi.UpdateMetadataAsync(ratingKey, metadataFields);

            var updatedFields = new List<string>();
            if (title != null) updatedFields.Add($"title: '{title}'");
            if (summary != null) updatedFields.Add($"summary: '{summary[..Math.Min(50, summary.Length)]}{(summary.Length > 50 ? "..." : "")}'");
            if (rating.HasValue) updatedFields.Add($"rating: {rating.Value:F1}");
            if (contentRating != null) updatedFields.Add($"content rating: '{contentRating}'");
            if (studio != null) updatedFields.Add($"studio: '{studio}'");
            if (year.HasValue) updatedFields.Add($"year: {year.Value}");

            return $"Metadata updated successfully for media item '{ratingKey}'. Updated fields: {string.Join(", ", updatedFields)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating metadata for {RatingKey}", ratingKey);
            return $"Error updating metadata: {ex.Message}";
        }
    }
}