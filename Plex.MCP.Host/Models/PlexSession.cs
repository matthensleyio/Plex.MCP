using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models;

public record PlexSession(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("bandwidth")] int Bandwidth,
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("User")] PlexUser User,
    [property: JsonPropertyName("Player")] PlexPlayer Player,
    [property: JsonPropertyName("Session")] PlexSessionInfo SessionInfo,
    [property: JsonPropertyName("TranscodeSession")] PlexTranscodeSession? TranscodeSession = null
);

public record PlexUser(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("uuid")] string? Uuid,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("recommendationsPlaylistId")] string? RecommendationsPlaylistId,
    [property: JsonPropertyName("thumb")] string? Thumb
);

public record PlexPlayer(
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("device")] string Device,
    [property: JsonPropertyName("machineIdentifier")] string MachineIdentifier,
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("platformVersion")] string PlatformVersion,
    [property: JsonPropertyName("product")] string Product,
    [property: JsonPropertyName("profile")] string Profile,
    [property: JsonPropertyName("remotePublicAddress")] string RemotePublicAddress,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("local")] bool Local,
    [property: JsonPropertyName("relayed")] bool Relayed,
    [property: JsonPropertyName("secure")] bool Secure,
    [property: JsonPropertyName("userID")] int UserId
);

public record PlexSessionInfo(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("bandwidth")] int Bandwidth,
    [property: JsonPropertyName("location")] string Location
);

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

public record PlexSessionsResponse(
    [property: JsonPropertyName("MediaContainer")] PlexSessionsMediaContainer MediaContainer
);

public record PlexSessionsMediaContainer(
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("Metadata")] List<PlexSession>? Sessions = null
);