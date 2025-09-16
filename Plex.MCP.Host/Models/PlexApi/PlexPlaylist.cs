using System.Text.Json.Serialization;
using Plex.MCP.Host.Models.Output;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexPlaylist(
    [property: JsonPropertyName("ratingKey")] string RatingKey,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("guid")] string Guid,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("titleSort")] string? TitleSort,
    [property: JsonPropertyName("summary")] string? Summary,
    [property: JsonPropertyName("smart")] bool Smart,
    [property: JsonPropertyName("playlistType")] string PlaylistType,
    [property: JsonPropertyName("composite")] string? Composite,
    [property: JsonPropertyName("icon")] string? Icon,
    [property: JsonPropertyName("viewCount")] int? ViewCount,
    [property: JsonPropertyName("lastViewedAt")] long? LastViewedAt,
    [property: JsonPropertyName("duration")] long? Duration,
    [property: JsonPropertyName("leafCount")] int LeafCount,
    [property: JsonPropertyName("addedAt")] long AddedAt,
    [property: JsonPropertyName("updatedAt")] long UpdatedAt
);