using Plex.MCP.Host.Models.PlexApi;
using System.Text.Json.Serialization;

namespace Plex.MCP.Host.Models.Output;

public record PlexLibrariesResponse(
    [property: JsonPropertyName("Directory")] List<PlexLibrary> Libraries
);