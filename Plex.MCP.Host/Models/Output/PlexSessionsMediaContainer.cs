using Plex.MCP.Host.Models.PlexApi;
using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexSessionsMediaContainer(
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("Metadata")] List<PlexSession>? Sessions = null
);