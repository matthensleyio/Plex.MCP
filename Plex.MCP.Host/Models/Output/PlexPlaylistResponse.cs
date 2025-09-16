using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexPlaylistResponse(
    [property: JsonPropertyName("MediaContainer")] PlexPlaylistMediaContainer MediaContainer
);