using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plex.MCP.Host.Models.Output;
using Plex.MCP.Host.Models.PlexApi;
using System.Text.Json;

namespace Plex.MCP.Host.Services;

public class PlexApiService : IPlexApiService
{
    private readonly ILogger<PlexApiService> _logger;
    private readonly string _baseUrl;
    private readonly string? _plexToken;
    private readonly HttpClient _httpClient;

    public PlexApiService(IConfiguration configuration, ILogger<PlexApiService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _baseUrl = configuration["Plex:ServerUrl"] ?? "http://localhost:32400";
        _plexToken = configuration["Plex:Token"];

        _logger.LogInformation("PlexApiService initializing with HttpClient for JSON - Base URL: {BaseUrl}, Token configured: {HasToken}", _baseUrl, !string.IsNullOrEmpty(_plexToken));

        _logger.LogInformation("PlexApiService initialized successfully with JSON support");
    }

    private async Task<T?> MakeApiRequestAsync<T>(string endpoint, string methodName, Dictionary<string, string>? queryParams = null) where T : class
    {
        _logger.LogInformation("Executing {MethodName}", methodName);
        var uriBuilder = new UriBuilder($"{_baseUrl}{endpoint}");
        var query = new List<string>();

        if (!string.IsNullOrEmpty(_plexToken))
        {
            query.Add($"X-Plex-Token={_plexToken}");
        }

        if (queryParams != null)
        {
            foreach (var param in queryParams)
            {
                query.Add($"{param.Key}={Uri.EscapeDataString(param.Value)}");
            }
        }

        if (query.Any())
        {
            uriBuilder.Query = string.Join("&", query);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
        request.Headers.Add("Accept", "application/json");

        LogRequest(request);

        try
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            LogResponse(response, content);

            response.EnsureSuccessStatusCode();

            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("{MethodName} received an empty response.", methodName);
                return null;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false
            };

            var fullResponse = JsonSerializer.Deserialize<JsonDocument>(content, jsonOptions);
            if (fullResponse == null || !fullResponse.RootElement.TryGetProperty("MediaContainer", out var mediaContainerElement))
            {
                _logger.LogWarning("No 'MediaContainer' found in the response for {MethodName}.", methodName);
                return null;
            }

            var mediaContainerJson = mediaContainerElement.GetRawText();
            var result = JsonSerializer.Deserialize<T>(mediaContainerJson, jsonOptions);

            _logger.LogInformation("{MethodName} executed successfully.", methodName);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request for {MethodName} failed.", methodName);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization for {MethodName} failed.", methodName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred in {MethodName}.", methodName);
            throw;
        }
    }

    private async Task MakeApiRequestAsync(string endpoint, string methodName, Dictionary<string, string>? queryParams = null)
    {
        _logger.LogInformation("Executing {MethodName}", methodName);
        var uriBuilder = new UriBuilder($"{_baseUrl}{endpoint}");
        var query = new List<string>();

        if (!string.IsNullOrEmpty(_plexToken))
        {
            query.Add($"X-Plex-Token={_plexToken}");
        }

        if (queryParams != null)
        {
            foreach (var param in queryParams)
            {
                query.Add($"{param.Key}={Uri.EscapeDataString(param.Value)}");
            }
        }

        if (query.Any())
        {
            uriBuilder.Query = string.Join("&", query);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
        request.Headers.Add("Accept", "application/json");

        LogRequest(request);

        try
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            LogResponse(response, content);

            response.EnsureSuccessStatusCode();

            _logger.LogInformation("{MethodName} executed successfully.", methodName);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request for {MethodName} failed.", methodName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred in {MethodName}.", methodName);
            throw;
        }
    }

    private void LogRequest(HttpRequestMessage request)
    {
        _logger.LogInformation("Request: {Method} {Url}", request.Method, request.RequestUri);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var headers = request.Headers.ToString().Trim();
            if (!string.IsNullOrEmpty(headers))
            {
                _logger.LogDebug("Request Headers:{NewLine}{Headers}", Environment.NewLine, headers);
            }
        }
    }

    private void LogResponse(HttpResponseMessage response, string content)
    {
        _logger.LogInformation("Response: {StatusCode} {ReasonPhrase}", (int)response.StatusCode, response.ReasonPhrase);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var headers = response.Headers.ToString().Trim();
            if (!string.IsNullOrEmpty(headers))
            {
                _logger.LogDebug("Response Headers:{NewLine}{Headers}", Environment.NewLine, headers);
            }

            if (!string.IsNullOrEmpty(content))
            {
                _logger.LogDebug("Response Body:{NewLine}{Body}", Environment.NewLine, content);
            }
            else
            {
                _logger.LogDebug("Response Body is empty.");
            }
        }
    }

    public async Task<PlexServerCapabilities?> GetServerCapabilitiesAsync()
    {
        return await MakeApiRequestAsync<PlexServerCapabilities>("/", nameof(GetServerCapabilitiesAsync));
    }

    public async Task<PlexLibrariesResponse?> GetLibrariesAsync()
    {
        var result = await MakeApiRequestAsync<PlexLibrariesResponse>("/library/sections", nameof(GetLibrariesAsync));
        if (result != null)
        {
            _logger.LogInformation("GetLibrariesAsync found {Count} libraries", result.Libraries?.Count ?? 0);
        }
        return result;
    }

    public async Task<PlexLibrary?> GetLibraryAsync(string sectionId)
    {
        var result = await MakeApiRequestAsync<PlexLibrariesResponse>($"/library/sections/{sectionId}", $"{nameof(GetLibraryAsync)} for section {sectionId}");
        var library = result?.Libraries?.FirstOrDefault();
        _logger.LogInformation("GetLibraryAsync completed - Found library: {Found}", library != null);
        return library;
    }

    public async Task<PlexMediaContainer?> GetLibraryItemsAsync(string sectionId, int start = 0, int size = 50)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["X-Plex-Container-Start"] = start.ToString(),
            ["X-Plex-Container-Size"] = size.ToString()
        };

        var result = await MakeApiRequestAsync<PlexMediaContainer>($"/library/sections/{sectionId}/all", $"{nameof(GetLibraryItemsAsync)} for section {sectionId}, start={start}, size={size}", queryParams);
        if (result != null)
        {
            _logger.LogInformation("GetLibraryItemsAsync found {Count} items", result.Metadata?.Count ?? 0);
        }
        return result;
    }

    public async Task<PlexMediaContainer?> GetRecentlyAddedAsync(int start = 0, int size = 50)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["X-Plex-Container-Start"] = start.ToString(),
            ["X-Plex-Container-Size"] = size.ToString()
        };

        var result = await MakeApiRequestAsync<PlexMediaContainer>("/library/recentlyAdded", $"{nameof(GetRecentlyAddedAsync)}, start={start}, size={size}", queryParams);
        if (result != null)
        {
            _logger.LogInformation("GetRecentlyAddedAsync found {Count} items", result.Metadata?.Count ?? 0);
        }
        return result;
    }

    public async Task<PlexSearchResponse?> SearchAsync(string query, int start = 0, int size = 50)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["query"] = query,
            ["X-Plex-Container-Start"] = start.ToString(),
            ["X-Plex-Container-Size"] = size.ToString()
        };

        var result = await MakeApiRequestAsync<PlexSearchResponse>("/search", $"{nameof(SearchAsync)} for query '{query}', start={start}, size={size}", queryParams);
        if (result != null)
        {
            _logger.LogInformation("SearchAsync found {Count} results", result.MediaContainer?.Results?.Count ?? 0);
        }
        return result;
    }

    public async Task<PlexMediaContainer?> SearchLibraryAsync(string sectionId, string query, int start = 0, int size = 50)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["query"] = query,
            ["sectionId"] = sectionId,
            ["X-Plex-Container-Start"] = start.ToString(),
            ["X-Plex-Container-Size"] = size.ToString()
        };

        var result = await MakeApiRequestAsync<PlexMediaContainer>("/search", $"{nameof(SearchLibraryAsync)} for section {sectionId}, query '{query}', start={start}, size={size}", queryParams);
        if (result != null)
        {
            _logger.LogInformation("SearchLibraryAsync found {Count} results", result.Metadata?.Count ?? 0);
        }
        return result;
    }

    public async Task<PlexMediaItem?> GetMetadataAsync(string ratingKey)
    {
        var container = await MakeApiRequestAsync<PlexMediaContainer>($"/library/metadata/{ratingKey}", $"{nameof(GetMetadataAsync)} for rating key {ratingKey}");
        var metadata = container?.Metadata?.FirstOrDefault();
        _logger.LogInformation("GetMetadataAsync completed - Found metadata: {Found}", metadata != null);
        return metadata;
    }

    public async Task<PlexHubResponse?> GetHubsAsync()
    {
        return await MakeApiRequestAsync<PlexHubResponse>("/hubs", nameof(GetHubsAsync));
    }

    public async Task<PlexSessionsResponse?> GetSessionsAsync()
    {
        return await MakeApiRequestAsync<PlexSessionsResponse>("/status/sessions", nameof(GetSessionsAsync));
    }

    public async Task<PlexPlaylistResponse?> GetPlaylistsAsync()
    {
        return await MakeApiRequestAsync<PlexPlaylistResponse>("/playlists", nameof(GetPlaylistsAsync));
    }

    public async Task<PlexPlaylistResponse?> GetPlaylistAsync(string playlistId)
    {
        return await MakeApiRequestAsync<PlexPlaylistResponse>($"/playlists/{playlistId}/items", $"{nameof(GetPlaylistAsync)} for playlist {playlistId}");
    }

    public async Task RefreshLibraryAsync(string sectionId)
    {
        await MakeApiRequestAsync($"/library/sections/{sectionId}/refresh", $"{nameof(RefreshLibraryAsync)} for section {sectionId}");
    }

    public async Task<string?> GetHashValueAsync(string url)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["url"] = url
        };

        try
        {
            var result = await MakeApiRequestAsync<JsonDocument>("/library/metadata", $"{nameof(GetHashValueAsync)} for URL {url}", queryParams);
            var hash = result?.RootElement.GetProperty("hash").GetString();
            return hash;
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("No hash property found in response for URL {Url}", url);
            return null;
        }
    }

    public async Task MarkAsPlayedAsync(string ratingKey)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["key"] = ratingKey,
            ["identifier"] = "com.plexapp.plugins.library"
        };

        await MakeApiRequestAsync("/:/scrobble", $"{nameof(MarkAsPlayedAsync)} for rating key {ratingKey}", queryParams);
    }

    public async Task MarkAsUnplayedAsync(string ratingKey)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["key"] = ratingKey,
            ["identifier"] = "com.plexapp.plugins.library"
        };

        await MakeApiRequestAsync("/:/unscrobble", $"{nameof(MarkAsUnplayedAsync)} for rating key {ratingKey}", queryParams);
    }

    public async Task UpdatePlayProgressAsync(string ratingKey, int time, string state = "stopped")
    {
        var queryParams = new Dictionary<string, string>
        {
            ["key"] = ratingKey,
            ["identifier"] = "com.plexapp.plugins.library",
            ["time"] = time.ToString(),
            ["state"] = state
        };

        await MakeApiRequestAsync("/:/progress", $"{nameof(UpdatePlayProgressAsync)} for rating key {ratingKey}, time={time}, state={state}", queryParams);
    }

    public async Task<string?> GetRawJsonAsync(string endpoint)
    {
        var uriBuilder = new UriBuilder($"{_baseUrl}{endpoint}");
        var query = new List<string>();

        if (!string.IsNullOrEmpty(_plexToken))
        {
            query.Add($"X-Plex-Token={_plexToken}");
        }

        if (query.Any())
        {
            uriBuilder.Query = string.Join("&", query);
        }

        var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
        request.Headers.Add("Accept", "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting raw JSON from {Endpoint}", endpoint);
            return null;
        }
    }
}