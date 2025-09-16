using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexMediaContainer(
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("allowSync")] bool AllowSync,
    [property: JsonPropertyName("identifier")] string Identifier,
    [property: JsonPropertyName("mediaTagPrefix")] string MediaTagPrefix,
    [property: JsonPropertyName("mediaTagVersion")] long MediaTagVersion,
    [property: JsonPropertyName("title1")] string Title1,
    [property: JsonPropertyName("Directory")] List<PlexLibrary>? Directories = null,
    [property: JsonPropertyName("Metadata")] List<PlexMediaItem>? Metadata = null,
    [property: JsonPropertyName("Video")] List<PlexMediaItem>? Videos = null,
    [property: JsonPropertyName("Track")] List<PlexMediaItem>? Tracks = null
);