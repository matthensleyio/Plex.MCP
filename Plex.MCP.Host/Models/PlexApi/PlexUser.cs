using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexUser(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("uuid")] string? Uuid,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("recommendationsPlaylistId")] string? RecommendationsPlaylistId,
    [property: JsonPropertyName("thumb")] string? Thumb
);