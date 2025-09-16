using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexSessionsResponse(
    [property: JsonPropertyName("MediaContainer")] PlexSessionsMediaContainer MediaContainer
);