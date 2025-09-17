using Plex.MCP.Host.Models.Output;
using Plex.MCP.Host.Models.PlexApi;

namespace Plex.MCP.Host.Services;

public interface IPlexApiService
{
    Task<PlexServerCapabilities?> GetServerCapabilitiesAsync();
    Task<PlexLibrariesResponse?> GetLibrariesAsync();
    Task<PlexLibrary?> GetLibraryAsync(string sectionId);
    Task<PlexMediaContainer?> GetLibraryItemsAsync(string sectionId, int start = 0, int size = 50);
    Task<PlexMediaContainer?> GetRecentlyAddedAsync(int start = 0, int size = 50);
    Task<PlexSearchResponse?> SearchAsync(string query, int start = 0, int size = 50);
    Task<PlexMediaContainer?> SearchLibraryAsync(string sectionId, string query, int start = 0, int size = 50);
    Task<PlexMediaItem?> GetMetadataAsync(string ratingKey);
    Task<PlexHubResponse?> GetHubsAsync();
    Task<PlexSessionsResponse?> GetSessionsAsync();
    Task<PlexPlaylistResponse?> GetPlaylistsAsync();
    Task<PlexPlaylistResponse?> GetPlaylistAsync(string playlistId);
    Task RefreshLibraryAsync(string sectionId);
    Task<string?> GetHashValueAsync(string url);
    Task MarkAsPlayedAsync(string ratingKey);
    Task MarkAsUnplayedAsync(string ratingKey);
    Task UpdatePlayProgressAsync(string ratingKey, int time, string state = "stopped");
    Task<string?> GetRawJsonAsync(string endpoint);
    Task UpdateMetadataAsync(string ratingKey, Dictionary<string, object> metadataFields);
}
