using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexHubResponse(
    [property: JsonPropertyName("MediaContainer")] PlexHubMediaContainer MediaContainer
);