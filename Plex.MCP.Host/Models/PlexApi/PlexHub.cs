using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexHub(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("hubIdentifier")] string HubIdentifier,
    [property: JsonPropertyName("context")] string? Context,
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("more")] bool More,
    [property: JsonPropertyName("style")] string? Style,
    [property: JsonPropertyName("promoted")] bool? Promoted,
    [property: JsonPropertyName("random")] bool? Random,
    [property: JsonPropertyName("Metadata")] List<PlexMediaItem>? Metadata = null,
    [property: JsonPropertyName("Directory")] List<PlexLibrary>? Directories = null
);