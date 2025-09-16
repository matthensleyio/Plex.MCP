using System.Text.Json.Serialization;
using System.Text.Json;

namespace Plex.MCP.Host.Models;

public record PlexMediaItem(
    [property: JsonPropertyName("ratingKey")] string RatingKey,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("parentRatingKey")] string? ParentRatingKey,
    [property: JsonPropertyName("grandparentRatingKey")] string? GrandparentRatingKey,
    [property: JsonPropertyName("parentGuid")] string? ParentGuid,
    [property: JsonPropertyName("grandparentGuid")] string? GrandparentGuid,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("parentTitle")] string? ParentTitle,
    [property: JsonPropertyName("grandparentTitle")] string? GrandparentTitle,
    [property: JsonPropertyName("titleSort")] string? TitleSort,
    [property: JsonPropertyName("summary")] string? Summary,
    [property: JsonPropertyName("index")] int? Index,
    [property: JsonPropertyName("parentIndex")] int? ParentIndex,
    [property: JsonPropertyName("rating")] float? Rating,
    [property: JsonPropertyName("year")] int? Year,
    [property: JsonPropertyName("thumb")] string? Thumb,
    [property: JsonPropertyName("art")] string? Art,
    [property: JsonPropertyName("parentThumb")] string? ParentThumb,
    [property: JsonPropertyName("grandparentThumb")] string? GrandparentThumb,
    [property: JsonPropertyName("grandparentArt")] string? GrandparentArt,
    [property: JsonPropertyName("duration")] long? Duration,
    [property: JsonPropertyName("originallyAvailableAt")] string? OriginallyAvailableAt,
    [property: JsonPropertyName("addedAt")] long AddedAt,
    [property: JsonPropertyName("updatedAt")] long UpdatedAt,
    [property: JsonPropertyName("audienceRating")] float? AudienceRating,
    [property: JsonPropertyName("audienceRatingImage")] string? AudienceRatingImage,
    [property: JsonPropertyName("chapterSource")] string? ChapterSource,
    [property: JsonPropertyName("contentRating")] string? ContentRating,
    [property: JsonPropertyName("ratingImage")] string? RatingImage,
    [property: JsonPropertyName("studio")] string? Studio,
    [property: JsonPropertyName("tagline")] string? Tagline,
    [property: JsonPropertyName("viewCount")] int? ViewCount,
    [property: JsonPropertyName("skipCount")] int? SkipCount,
    [property: JsonPropertyName("lastViewedAt")] long? LastViewedAt,
    [property: JsonPropertyName("librarySectionTitle")] string? LibrarySectionTitle,
    [property: JsonPropertyName("librarySectionID")] int? LibrarySectionId,
    [property: JsonPropertyName("librarySectionKey")] string? LibrarySectionKey,
    [property: JsonPropertyName("playlistType")] string? PlaylistType,
    [property: JsonPropertyName("composite")] string? Composite,
    [property: JsonPropertyName("leafCount")] int? LeafCount,
    [property: JsonPropertyName("childCount")] int? ChildCount,
    [property: JsonPropertyName("skipChildren")] bool? SkipChildren,
    [property: JsonPropertyName("Genre")] List<PlexGenre>? Genres = null,
    [property: JsonPropertyName("Country")] List<PlexCountry>? Countries = null,
    [property: JsonPropertyName("Director")] List<PlexDirector>? Directors = null,
    [property: JsonPropertyName("Writer")] List<PlexWriter>? Writers = null,
    [property: JsonPropertyName("Producer")] List<PlexProducer>? Producers = null,
    [property: JsonPropertyName("Role")] List<PlexRole>? Roles = null,
    [property: JsonPropertyName("Similar")] List<PlexSimilar>? Similar = null,
    [property: JsonPropertyName("Collection")] List<PlexCollection>? Collections = null,
    [property: JsonPropertyName("Label")] List<PlexLabel>? Labels = null,
    [property: JsonPropertyName("Media")] List<PlexMedia>? Media = null,
    [property: JsonPropertyName("guid")] string? GuidString = null,
    [property: JsonPropertyName("Guid")] List<PlexGuidItem>? ExternalGuids = null
);

public record PlexGenre(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexCountry(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexDirector(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexWriter(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexProducer(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexRole(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("role")] string? Role,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexSimilar(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexCollection(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexLabel(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("filter")] string Filter,
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("count")] int? Count
);

public record PlexMedia(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("duration")] long Duration,
    [property: JsonPropertyName("bitrate")] int? Bitrate,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("height")] int? Height,
    [property: JsonPropertyName("aspectRatio")] float? AspectRatio,
    [property: JsonPropertyName("audioChannels")] int? AudioChannels,
    [property: JsonPropertyName("audioCodec")] string? AudioCodec,
    [property: JsonPropertyName("videoCodec")] string? VideoCodec,
    [property: JsonPropertyName("videoResolution")] string? VideoResolution,
    [property: JsonPropertyName("container")] string Container,
    [property: JsonPropertyName("videoFrameRate")] string? VideoFrameRate,
    [property: JsonPropertyName("optimizedForStreaming")] int? OptimizedForStreaming,
    [property: JsonPropertyName("audioProfile")] string? AudioProfile,
    [property: JsonPropertyName("has64bitOffsets")] bool? Has64bitOffsets,
    [property: JsonPropertyName("videoProfile")] string? VideoProfile,
    [property: JsonPropertyName("Part")] List<PlexPart>? Parts = null
);

public record PlexPart(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("duration")] long Duration,
    [property: JsonPropertyName("file")] string File,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("audioProfile")] string? AudioProfile,
    [property: JsonPropertyName("container")] string Container,
    [property: JsonPropertyName("has64bitOffsets")] bool? Has64bitOffsets,
    [property: JsonPropertyName("optimizedForStreaming")] bool? OptimizedForStreaming,
    [property: JsonPropertyName("videoProfile")] string? VideoProfile,
    [property: JsonPropertyName("Stream")] List<PlexStream>? Streams = null
);

public record PlexStream(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("streamType")] int StreamType,
    [property: JsonPropertyName("default")] bool? Default,
    [property: JsonPropertyName("codec")] string Codec,
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("bitrate")] int? Bitrate,
    [property: JsonPropertyName("language")] string? Language,
    [property: JsonPropertyName("languageTag")] string? LanguageTag,
    [property: JsonPropertyName("languageCode")] string? LanguageCode,
    [property: JsonPropertyName("bitDepth")] int? BitDepth,
    [property: JsonPropertyName("chromaLocation")] string? ChromaLocation,
    [property: JsonPropertyName("chromaSubsampling")] string? ChromaSubsampling,
    [property: JsonPropertyName("codedHeight")] int? CodedHeight,
    [property: JsonPropertyName("codedWidth")] int? CodedWidth,
    [property: JsonPropertyName("colorRange")] string? ColorRange,
    [property: JsonPropertyName("frameRate")] float? FrameRate,
    [property: JsonPropertyName("height")] int? Height,
    [property: JsonPropertyName("level")] int? Level,
    [property: JsonPropertyName("profile")] string? Profile,
    [property: JsonPropertyName("refFrames")] int? RefFrames,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("displayTitle")] string? DisplayTitle,
    [property: JsonPropertyName("extendedDisplayTitle")] string? ExtendedDisplayTitle,
    [property: JsonPropertyName("selected")] bool? Selected,
    [property: JsonPropertyName("channels")] int? Channels,
    [property: JsonPropertyName("audioChannelLayout")] string? AudioChannelLayout,
    [property: JsonPropertyName("samplingRate")] int? SamplingRate,
    [property: JsonPropertyName("title")] string? Title
);

public record PlexGuidItem(
    [property: JsonPropertyName("id")] string Id
);