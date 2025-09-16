using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexCollection(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);