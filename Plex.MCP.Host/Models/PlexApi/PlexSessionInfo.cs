using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexSessionInfo(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("bandwidth")] int Bandwidth,
    [property: JsonPropertyName("location")] string Location
);