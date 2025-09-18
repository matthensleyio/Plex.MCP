using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Plex.MCP.Host.Models;
using Plex.MCP.Host.Models.Output;
using Plex.MCP.Host.Models.PlexApi;
using Plex.MCP.Host.Services;
using Plex.MCP.Host.Mcp;
using System.ComponentModel;
using System.Text.Json;

namespace Plex.MCP.Host.Tools;

[McpServerToolType]
public class ServerTools
{
    private readonly IPlexApiService _plexApi;
    private readonly ILogger<ServerTools> _logger;
    private readonly McpDispatcher _dispatcher;

    public ServerTools(IPlexApiService plexApi, ILogger<ServerTools> logger, McpDispatcher dispatcher)
    {
        _plexApi = plexApi;
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [McpServerTool, Description("Get Plex server capabilities and information")]
    public Task<McpResponse<PlexServerCapabilities>> GetServerCapabilitiesAsync()
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            var capabilities = await _plexApi.GetServerCapabilitiesAsync();
            if (capabilities == null)
                throw new InvalidOperationException("Unable to retrieve server capabilities.");

            return capabilities;
        });
    }

    [McpServerTool, Description("Get current Plex server sessions")]
    public Task<McpResponse<PlexSessionsResponse>> GetSessionsAsync()
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            var sessions = await _plexApi.GetSessionsAsync();
            if (sessions == null)
                throw new InvalidOperationException("No active sessions found.");

            return sessions;
        });
    }

    [McpServerTool, Description("Get hash value for a local file URL")]
    public Task<McpResponse<string>> GetHashValueAsync(
        [Description("Local file URL to get hash for")] string url)
    {
        return _dispatcher.DispatchAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL is required.", nameof(url));

            var hash = await _plexApi.GetHashValueAsync(url);
            if (string.IsNullOrEmpty(hash))
                throw new InvalidOperationException($"Unable to get hash for URL: {url}");

            return hash;
        });
    }
}