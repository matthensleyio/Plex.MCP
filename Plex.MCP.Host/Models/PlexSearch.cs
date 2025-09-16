using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models;

public record PlexSearchResponse(
    [property: JsonPropertyName("MediaContainer")] PlexSearchMediaContainer MediaContainer
);

public record PlexSearchMediaContainer(
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("identifier")] string Identifier,
    [property: JsonPropertyName("mediaTagPrefix")] string MediaTagPrefix,
    [property: JsonPropertyName("mediaTagVersion")] long MediaTagVersion,
    [property: JsonPropertyName("Metadata")] List<PlexMediaItem>? Results = null,
    [property: JsonPropertyName("Provider")] List<PlexSearchProvider>? Providers = null,
    [property: JsonPropertyName("SearchResult")] List<PlexSearchResult>? SearchResults = null
);

public record PlexSearchProvider(
    [property: JsonPropertyName("identifier")] string Identifier,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("types")] string Types
);

public record PlexSearchResult(
    [property: JsonPropertyName("score")] float Score,
    [property: JsonPropertyName("Metadata")] PlexMediaItem Metadata
);

public record PlexHubResponse(
    [property: JsonPropertyName("MediaContainer")] PlexHubMediaContainer MediaContainer
);

public record PlexHubMediaContainer(
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("identifier")] string Identifier,
    [property: JsonPropertyName("mediaTagPrefix")] string MediaTagPrefix,
    [property: JsonPropertyName("mediaTagVersion")] long MediaTagVersion,
    [property: JsonPropertyName("title1")] string Title1,
    [property: JsonPropertyName("Hub")] List<PlexHub>? Hubs = null
);

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