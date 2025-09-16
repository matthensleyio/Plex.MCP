using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Plex.MCP.Host.Models;
using Plex.MCP.Host.Services;
using System.ComponentModel;
using System.Text.Json;

namespace Plex.MCP.Host.Tools;

[McpServerToolType]
public class ServerTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<ServerTools> _logger;

    public ServerTools(IPlexApiService plexApi, ILogger<ServerTools> logger)
    {
        _plexApi = plexApi;
        _logger = logger;
    }

    [McpServerTool, Description("Get Plex server capabilities and information")]
    public async Task<string> GetServerCapabilitiesAsync()
    {
        try
        {
            var capabilities = await _plexApi.GetServerCapabilitiesAsync();
            if (capabilities == null)
                return "Unable to retrieve server capabilities.";

            return JsonSerializer.Serialize(capabilities, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting server capabilities");
            return $"Error retrieving server capabilities: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get current Plex server sessions")]
    public async Task<string> GetSessionsAsync()
    {
        try
        {
            var sessions = await _plexApi.GetSessionsAsync();
            if (sessions == null)
                return "No active sessions found.";

            return JsonSerializer.Serialize(sessions, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions");
            return $"Error retrieving sessions: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get hash value for a local file URL")]
    public async Task<string> GetHashValueAsync(
        [Description("Local file URL to get hash for")] string url)
    {
        try
        {
            var hash = await _plexApi.GetHashValueAsync(url);
            if (string.IsNullOrEmpty(hash))
                return $"Unable to get hash for URL: {url}";

            return hash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hash value for {Url}", url);
            return $"Error retrieving hash value: {ex.Message}";
        }
    }
}