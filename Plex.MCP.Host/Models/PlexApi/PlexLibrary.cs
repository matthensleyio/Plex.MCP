using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexLibrary(
    [property: JsonPropertyName("allowSync")] bool AllowSync,
    [property: JsonPropertyName("art")] string Art,
    [property: JsonPropertyName("composite")] string Composite,
    [property: JsonPropertyName("filters")] bool Filters,
    [property: JsonPropertyName("refreshing")] bool Refreshing,
    [property: JsonPropertyName("thumb")] string Thumb,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("agent")] string Agent,
    [property: JsonPropertyName("scanner")] string Scanner,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("uuid")] string Uuid,
    [property: JsonPropertyName("updatedAt")] long UpdatedAt,
    [property: JsonPropertyName("createdAt")] long CreatedAt,
    [property: JsonPropertyName("scannedAt")] long ScannedAt,
    [property: JsonPropertyName("content")] bool Content,
    [property: JsonPropertyName("directory")] bool Directory,
    [property: JsonPropertyName("contentChangedAt")] long ContentChangedAt,
    [property: JsonPropertyName("hidden")] int Hidden,
    [property: JsonPropertyName("Location")] List<PlexLocation>? Locations = null
);