using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.PlexApi;

public record PlexPlayer(
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("device")] string Device,
    [property: JsonPropertyName("machineIdentifier")] string MachineIdentifier,
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("platformVersion")] string PlatformVersion,
    [property: JsonPropertyName("product")] string Product,
    [property: JsonPropertyName("profile")] string Profile,
    [property: JsonPropertyName("remotePublicAddress")] string RemotePublicAddress,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("local")] bool Local,
    [property: JsonPropertyName("relayed")] bool Relayed,
    [property: JsonPropertyName("secure")] bool Secure,
    [property: JsonPropertyName("userID")] int UserId
);