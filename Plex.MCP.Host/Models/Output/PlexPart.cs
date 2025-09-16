using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexPart(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("duration")] long Duration,
    [property: JsonPropertyName("file")] string File,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("audioProfile")] string? AudioProfile,
    [property: JsonPropertyName("container")] string Container,
    [property: JsonPropertyName("has64bitOffsets")] bool? Has64bitOffsets,
    [property: JsonPropertyName("optimizedForStreaming")] bool? OptimizedForStreaming,
    [property: JsonPropertyName("videoProfile")] string? VideoProfile,
    [property: JsonPropertyName("Stream")] List<PlexStream>? Streams = null
);