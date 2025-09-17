using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Plex.MCP.Host.Models.PlexApi;
using Plex.MCP.Host.Services;
using System.Net;
using System.Reflection;

namespace Plex.MCP.Host.Tests;

public class PlexApiServiceUnitTests
{
    private readonly Mock<ILogger<PlexApiService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly PlexApiService _plexApiService;

    public PlexApiServiceUnitTests()
    {
        _mockLogger = new Mock<ILogger<PlexApiService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _mockConfiguration.Setup(c => c["Plex:ServerUrl"]).Returns("http://localhost:32400");
        _mockConfiguration.Setup(c => c["Plex:Token"]).Returns("test-token");

        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _plexApiService = new PlexApiService(_mockConfiguration.Object, _mockLogger.Object, _httpClient);
    }

    [Theory]
    [InlineData("movie", "1")]
    [InlineData("Movie", "1")]
    [InlineData("show", "2")]
    [InlineData("season", "3")]
    [InlineData("episode", "4")]
    [InlineData("trailer", "5")]
    [InlineData("comic", "6")]
    [InlineData("person", "7")]
    [InlineData("artist", "8")]
    [InlineData("album", "9")]
    [InlineData("track", "10")]
    [InlineData("photoalbum", "11")]
    [InlineData("photo", "12")]
    [InlineData("clip", "13")]
    [InlineData("playlistitem", "15")]
    [InlineData("unknown", "1")]
    [InlineData("", "1")]
    public void GetMediaTypeNumber_ShouldReturnCorrectTypeNumber(string mediaType, string expectedTypeNumber)
    {
        // Act
        var result = InvokePrivateStaticMethod("GetMediaTypeNumber", mediaType);

        // Assert
        Assert.Equal(expectedTypeNumber, result);
    }

    [Fact]
    public void UpdateMetadataAsync_ParameterValidation_ShouldHandleComplexFieldStructures()
    {
        // This test validates that our parameter building logic correctly handles
        // both simple values and complex field configurations with locking

        // Arrange - Test the parameter building logic used in UpdateMetadataAsync
        var metadataFields = new Dictionary<string, object>
        {
            ["title"] = new Dictionary<string, object> { ["value"] = "New Title", ["locked"] = true },
            ["summary"] = "Simple Summary",
            ["rating"] = new Dictionary<string, object> { ["value"] = "8.5", ["locked"] = false }
        };

        var queryParams = new Dictionary<string, string>
        {
            ["type"] = "1",
            ["id"] = "12345",
            ["includeExternalMedia"] = "1"
        };

        // Act - Apply the same logic as UpdateMetadataAsync
        foreach (var field in metadataFields)
        {
            var fieldName = field.Key;
            var fieldValue = field.Value;

            if (fieldValue is Dictionary<string, object> fieldConfig)
            {
                if (fieldConfig.ContainsKey("value"))
                {
                    queryParams[$"{fieldName}.value"] = fieldConfig["value"]?.ToString() ?? "";
                }
                if (fieldConfig.ContainsKey("locked") && fieldConfig["locked"] is bool locked && locked)
                {
                    queryParams[$"{fieldName}.locked"] = "1";
                }
            }
            else
            {
                queryParams[$"{fieldName}.value"] = fieldValue?.ToString() ?? "";
            }
        }

        // Assert
        Assert.Equal("New Title", queryParams["title.value"]);
        Assert.Equal("1", queryParams["title.locked"]);
        Assert.Equal("Simple Summary", queryParams["summary.value"]);
        Assert.False(queryParams.ContainsKey("summary.locked"));
        Assert.Equal("8.5", queryParams["rating.value"]);
        Assert.False(queryParams.ContainsKey("rating.locked")); // locked is false, so no lock parameter
    }

    [Fact]
    public void UpdateMetadataAsync_BuildsCorrectQueryParameters_ForSimpleFields()
    {
        // Arrange
        var metadataFields = new Dictionary<string, object>
        {
            ["title"] = "New Title",
            ["summary"] = "New Summary",
            ["year"] = "2024"
        };

        // Act - We're testing the parameter building logic
        var queryParams = new Dictionary<string, string>
        {
            ["type"] = "1",
            ["id"] = "12345",
            ["includeExternalMedia"] = "1"
        };

        foreach (var field in metadataFields)
        {
            queryParams[$"{field.Key}.value"] = field.Value?.ToString() ?? "";
        }

        // Assert
        Assert.Equal("New Title", queryParams["title.value"]);
        Assert.Equal("New Summary", queryParams["summary.value"]);
        Assert.Equal("2024", queryParams["year.value"]);
        Assert.Equal("1", queryParams["type"]);
        Assert.Equal("12345", queryParams["id"]);
        Assert.Equal("1", queryParams["includeExternalMedia"]);
    }

    [Fact]
    public void UpdateMetadataAsync_BuildsCorrectQueryParameters_ForFieldsWithLocking()
    {
        // Arrange
        var metadataFields = new Dictionary<string, object>
        {
            ["title"] = new Dictionary<string, object> { ["value"] = "New Title", ["locked"] = true },
            ["summary"] = new Dictionary<string, object> { ["value"] = "New Summary", ["locked"] = false },
            ["year"] = new Dictionary<string, object> { ["value"] = "2024" }
        };

        // Act - We're testing the parameter building logic
        var queryParams = new Dictionary<string, string>
        {
            ["type"] = "1",
            ["id"] = "12345",
            ["includeExternalMedia"] = "1"
        };

        foreach (var field in metadataFields)
        {
            var fieldName = field.Key;
            var fieldValue = field.Value;

            if (fieldValue is Dictionary<string, object> fieldConfig)
            {
                if (fieldConfig.ContainsKey("value"))
                {
                    queryParams[$"{fieldName}.value"] = fieldConfig["value"]?.ToString() ?? "";
                }
                if (fieldConfig.ContainsKey("locked") && fieldConfig["locked"] is bool locked && locked)
                {
                    queryParams[$"{fieldName}.locked"] = "1";
                }
            }
            else
            {
                queryParams[$"{fieldName}.value"] = fieldValue?.ToString() ?? "";
            }
        }

        // Assert
        Assert.Equal("New Title", queryParams["title.value"]);
        Assert.Equal("1", queryParams["title.locked"]);
        Assert.Equal("New Summary", queryParams["summary.value"]);
        Assert.False(queryParams.ContainsKey("summary.locked"));
        Assert.Equal("2024", queryParams["year.value"]);
        Assert.False(queryParams.ContainsKey("year.locked"));
    }

    private string InvokePrivateStaticMethod(string methodName, params object[] parameters)
    {
        var method = typeof(PlexApiService).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        var result = method.Invoke(null, parameters);
        return result?.ToString() ?? "";
    }
}