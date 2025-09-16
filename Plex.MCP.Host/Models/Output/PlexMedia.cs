using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexMedia(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("duration")] long Duration,
    [property: JsonPropertyName("bitrate")] int? Bitrate,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("height")] int? Height,
    [property: JsonPropertyName("aspectRatio")] float? AspectRatio,
    [property: JsonPropertyName("audioChannels")] int? AudioChannels,
    [property: JsonPropertyName("audioCodec")] string? AudioCodec,
    [property: JsonPropertyName("videoCodec")] string? VideoCodec,
    [property: JsonPropertyName("videoResolution")] string? VideoResolution,
    [property: JsonPropertyName("container")] string Container,
    [property: JsonPropertyName("videoFrameRate")] string? VideoFrameRate,
    [property: JsonPropertyName("optimizedForStreaming")] int? OptimizedForStreaming,
    [property: JsonPropertyName("audioProfile")] string? AudioProfile,
    [property: JsonPropertyName("has64bitOffsets")] bool? Has64bitOffsets,
    [property: JsonPropertyName("videoProfile")] string? VideoProfile,
    [property: JsonPropertyName("Part")] List<PlexPart>? Parts = null
);