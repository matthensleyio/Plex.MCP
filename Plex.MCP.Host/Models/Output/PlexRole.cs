using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexRole(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("role")] string? Role,
    [property: JsonPropertyName("count")] int? Count
);