using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexServerInfo(
    [property: JsonPropertyName("friendlyName")] string FriendlyName,
    [property: JsonPropertyName("machineIdentifier")] string MachineIdentifier,
    [property: JsonPropertyName("size")] string Size,
    [property: JsonPropertyName("version")] string Version
);