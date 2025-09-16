using Plex.MCP.Host.Models.PlexApi;
using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexPlaylistMediaContainer(
    [property: JsonPropertyName("size")] int Size,
    [property: JsonPropertyName("leafCountRequested")] int? LeafCountRequested,
    [property: JsonPropertyName("leafCountAdded")] int? LeafCountAdded,
    [property: JsonPropertyName("identifier")] string Identifier,
    [property: JsonPropertyName("mediaTagPrefix")] string MediaTagPrefix,
    [property: JsonPropertyName("mediaTagVersion")] long MediaTagVersion,
    [property: JsonPropertyName("ratingKey")] string? RatingKey,
    [property: JsonPropertyName("key")] string? Key,
    [property: JsonPropertyName("guid")] string? Guid,
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("titleSort")] string? TitleSort,
    [property: JsonPropertyName("summary")] string? Summary,
    [property: JsonPropertyName("smart")] bool? Smart,
    [property: JsonPropertyName("playlistType")] string? PlaylistType,
    [property: JsonPropertyName("composite")] string? Composite,
    [property: JsonPropertyName("icon")] string? Icon,
    [property: JsonPropertyName("viewCount")] int? ViewCount,
    [property: JsonPropertyName("lastViewedAt")] long? LastViewedAt,
    [property: JsonPropertyName("duration")] long? Duration,
    [property: JsonPropertyName("leafCount")] int? LeafCount,
    [property: JsonPropertyName("addedAt")] long? AddedAt,
    [property: JsonPropertyName("updatedAt")] long? UpdatedAt,
    [property: JsonPropertyName("Metadata")] List<PlexMediaItem>? Items = null,
    [property: JsonPropertyName("Playlist")] List<PlexPlaylist>? Playlists = null
);