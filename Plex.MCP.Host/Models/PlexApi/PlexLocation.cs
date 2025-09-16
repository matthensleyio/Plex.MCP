using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexLocation(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("path")] string Path
);