using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexSearchResult(
    [property: JsonPropertyName("score")] float Score,
    [property: JsonPropertyName("Metadata")] PlexMediaItem Metadata
);