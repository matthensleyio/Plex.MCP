using Plex.MCP.Host.Models.PlexApi;
using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexHubMediaContainer(
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("identifier")] string Identifier,
    [property: JsonPropertyName("mediaTagPrefix")] string MediaTagPrefix,
    [property: JsonPropertyName("mediaTagVersion")] long MediaTagVersion,
    [property: JsonPropertyName("title1")] string Title1,
    [property: JsonPropertyName("Hub")] List<PlexHub>? Hubs = null
);