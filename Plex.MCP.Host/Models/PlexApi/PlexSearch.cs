using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexSearchResponse(
    [property: JsonPropertyName("MediaContainer")] PlexSearchMediaContainer MediaContainer
);