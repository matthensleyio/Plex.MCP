# Plex MCP Server

A Model Context Protocol (MCP) server that provides tools to interact with your Plex Media Server. This server exposes various Plex functionalities as MCP tools that can be used by MCP-compatible clients like Claude Desktop.

## Features

This MCP server provides tools for:

### Library Management
- `GetLibrariesAsync` - Get all library sections from the Plex server.
- `GetLibraryAsync` - Get details about a specific library section.
- `GetLibraryItemsAsync` - Get items from a specific library section.
- `GetRecentlyAddedAsync` - Get recently added content across all libraries.
- `RefreshLibraryAsync` - Start a metadata refresh for a specific library section.
- `SearchLibraryAsync` - Search for content within a specific library.

### Media Item Management
- `GetMetadataAsync` - Get detailed metadata for a specific media item.
- `MarkAsPlayedAsync` - Mark a media item (like a movie or episode) as played.
- `MarkAsUnplayedAsync` - Mark a media item as unplayed.
- `UpdatePlayProgressAsync` - Update the playback progress for a media item.

### Playlist Management
- `GetPlaylistsAsync` - Get all playlists from the Plex server.
- `GetPlaylistAsync` - Get details and items from a specific playlist.

### Search
- `SearchAsync` - Perform a global search for content across all libraries.
- `GetHubsAsync` - Get discovery hubs, which power the "Recommended" sections in Plex clients.

### Server & Session Management
- `GetServerCapabilitiesAsync` - Get information about the Plex server's capabilities.
- `GetSessionsAsync` - Get details about current playback sessions on the server.
- `GetHashValueAsync` - Get a hash value for a local file URL (used for matching with Plex).

## Setup

1.  **Find Your Plex Token**
    *   Log in to your Plex account in your web browser.
    *   Follow the instructions in this official Plex support article to find your token: [Finding an authentication token / X-Plex-Token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/)

2.  **Find Your Plex Server URL**
    *   This is typically the local IP address of the machine running your Plex server, followed by port `32400`. For example: `http://localhost:32400`.

3.  **Build and Run the Server**
    *   Open a terminal in the repository root.
    *   Build the project:
        ```bash
        dotnet build
        ```
    *   Run the server, providing your credentials. Note that from the root directory, you must specify the project file to run.
        ```bash
        dotnet run --project Plex.MCP.Host/Plex.MCP.Host.csproj -- --plex-server-url "http://your-plex-server:32400" --plex-token "your-plex-token"
        ```

## Claude Desktop Integration

To use this MCP server with Claude Desktop, add it to your Claude Desktop configuration file.

### Configuration Example

Add the following to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "plex": {
      "command": "dotnet",
      "args": [
        "run",
        "--project", "C:\\path\\to\\your\\Plex.MCP.Host\\Plex.MCP.Host.csproj",
        "--",
        "--plex-server-url", "http://your-plex-server:32400",
        "--plex-token", "your-plex-token"
      ],
      "icon": "file://C:\\path\\to\\your\\Plex.MCP.Host\\Assets\\plex-logo.jpg"
    }
  }
}
```

### Setup Steps
1.  Replace `C:\\path\\to\\your\\` with the actual, absolute path to this project on your machine.
2.  Replace `http://your-plex-server:32400` with your actual Plex Server URL.
3.  Replace `your-plex-token` with your actual X-Plex-Token.
4.  Save the configuration file and restart Claude Desktop.

## Dependencies

-   .NET 9.0
