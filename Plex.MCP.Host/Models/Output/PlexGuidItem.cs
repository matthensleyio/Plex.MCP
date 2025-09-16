using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexGuidItem(
    [property: JsonPropertyName("id")] string Id
);