using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexSearchMediaContainer(
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("identifier")] string Identifier,
    [property: JsonPropertyName("mediaTagPrefix")] string MediaTagPrefix,
    [property: JsonPropertyName("mediaTagVersion")] long MediaTagVersion,
    [property: JsonPropertyName("Metadata")] List<PlexMediaItem>? Results = null,
    [property: JsonPropertyName("Provider")] List<PlexSearchProvider>? Providers = null,
    [property: JsonPropertyName("SearchResult")] List<PlexSearchResult>? SearchResults = null
);