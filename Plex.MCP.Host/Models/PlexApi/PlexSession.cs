using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexSession(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("bandwidth")] int Bandwidth,
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("User")] PlexUser User,
    [property: JsonPropertyName("Player")] PlexPlayer Player,
    [property: JsonPropertyName("Session")] PlexSessionInfo SessionInfo,
    [property: JsonPropertyName("TranscodeSession")] PlexTranscodeSession? TranscodeSession = null
);