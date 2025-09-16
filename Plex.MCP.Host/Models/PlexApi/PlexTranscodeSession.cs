using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexTranscodeSession(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("throttled")] bool Throttled,
    [property: JsonPropertyName("complete")] bool Complete,
    [property: JsonPropertyName("progress")] float Progress,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("speed")] float Speed,
    [property: JsonPropertyName("error")] bool Error,
    [property: JsonPropertyName("duration")] long Duration,
    [property: JsonPropertyName("remaining")] int Remaining,
    [property: JsonPropertyName("context")] string Context,
    [property: JsonPropertyName("sourceVideoCodec")] string SourceVideoCodec,
    [property: JsonPropertyName("sourceAudioCodec")] string SourceAudioCodec,
    [property: JsonPropertyName("videoDecision")] string VideoDecision,
    [property: JsonPropertyName("audioDecision")] string AudioDecision,
    [property: JsonPropertyName("protocol")] string Protocol,
    [property: JsonPropertyName("container")] string Container,
    [property: JsonPropertyName("videoCodec")] string VideoCodec,
    [property: JsonPropertyName("audioCodec")] string AudioCodec,
    [property: JsonPropertyName("audioChannels")] int AudioChannels,
    [property: JsonPropertyName("transcodeHwRequested")] bool TranscodeHwRequested,
    [property: JsonPropertyName("transcodeHwFullPipeline")] bool TranscodeHwFullPipeline,
    [property: JsonPropertyName("transcodeHwEncode")] string TranscodeHwEncode,
    [property: JsonPropertyName("transcodeHwDecode")] string TranscodeHwDecode,
    [property: JsonPropertyName("transcodeHwDecodeName")] string TranscodeHwDecodeName,
    [property: JsonPropertyName("transcodeHwEncodeName")] string TranscodeHwEncodeName,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("maxOffsetAvailable")] long MaxOffsetAvailable,
    [property: JsonPropertyName("minOffsetAvailable")] long MinOffsetAvailable
);