using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexStream(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("streamType")] int StreamType,
    [property: JsonPropertyName("default")] bool? Default,
    [property: JsonPropertyName("codec")] string Codec,
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("bitrate")] int? Bitrate,
    [property: JsonPropertyName("language")] string? Language,
    [property: JsonPropertyName("languageTag")] string? LanguageTag,
    [property: JsonPropertyName("languageCode")] string? LanguageCode,
    [property: JsonPropertyName("bitDepth")] int? BitDepth,
    [property: JsonPropertyName("chromaLocation")] string? ChromaLocation,
    [property: JsonPropertyName("chromaSubsampling")] string? ChromaSubsampling,
    [property: JsonPropertyName("codedHeight")] int? CodedHeight,
    [property: JsonPropertyName("codedWidth")] int? CodedWidth,
    [property: JsonPropertyName("colorRange")] string? ColorRange,
    [property: JsonPropertyName("frameRate")] float? FrameRate,
    [property: JsonPropertyName("height")] int? Height,
    [property: JsonPropertyName("level")] int? Level,
    [property: JsonPropertyName("profile")] string? Profile,
    [property: JsonPropertyName("refFrames")] int? RefFrames,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("displayTitle")] string? DisplayTitle,
    [property: JsonPropertyName("extendedDisplayTitle")] string? ExtendedDisplayTitle,
    [property: JsonPropertyName("selected")] bool? Selected,
    [property: JsonPropertyName("channels")] int? Channels,
    [property: JsonPropertyName("audioChannelLayout")] string? AudioChannelLayout,
    [property: JsonPropertyName("samplingRate")] int? SamplingRate,
    [property: JsonPropertyName("title")] string? Title
);