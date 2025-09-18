using Microsoft.Extensions.Logging;
using Moq;
using Plex.MCP.Host.Services;
using Plex.MCP.Host.Tools;
using Plex.MCP.Host.Mcp;

namespace Plex.MCP.Host.Tests;

public class MediaToolsUnitTests
{
    private readonly Mock<IPlexApiService> _mockPlexApiService;
    private readonly Mock<ILogger<MediaTools>> _mockLogger;
    private readonly McpDispatcher _dispatcher;
    private readonly MediaTools _mediaTools;

    public MediaToolsUnitTests()
    {
        _mockPlexApiService = new Mock<IPlexApiService>();
        _mockLogger = new Mock<ILogger<MediaTools>>();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var dispatcherLogger = loggerFactory.CreateLogger<McpDispatcher>();
        _dispatcher = new McpDispatcher(dispatcherLogger);
        _mediaTools = new MediaTools(_mockPlexApiService.Object, _mockLogger.Object, _dispatcher);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithNoFields_ShouldReturnErrorMessage()
    {
        // Arrange
        var ratingKey = "12345";

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Error);
        Assert.Contains("No metadata fields provided to update", response.Error.Message);
        _mockPlexApiService.Verify(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Never);
    }

    [Theory]
    [InlineData(-1.0f)]
    [InlineData(10.1f)]
    [InlineData(15.0f)]
    public async Task UpdateMediaMetadataAsync_WithInvalidRating_ShouldReturnErrorMessage(float invalidRating)
    {
        // Arrange
        var ratingKey = "12345";

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, rating: invalidRating);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Error);
        Assert.Contains("Rating must be between 0.0 and 10.0", response.Error.Message);
        _mockPlexApiService.Verify(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Never);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(5.5f)]
    [InlineData(10.0f)]
    public async Task UpdateMediaMetadataAsync_WithValidRating_ShouldCallService(float validRating)
    {
        // Arrange
        var ratingKey = "12345";
        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, rating: validRating);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.Contains($"rating: {validRating:F1}", response.Result);
        _mockPlexApiService.Verify(x => x.UpdateMetadataAsync(ratingKey, It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithTitle_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var ratingKey = "12345";
        var title = "New Movie Title";
        Dictionary<string, object>? capturedMetadata = null;

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Callback<string, Dictionary<string, object>>((key, metadata) => capturedMetadata = metadata)
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, title: title);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.Contains($"title: '{title}'", response.Result);
        Assert.NotNull(capturedMetadata);
        Assert.True(capturedMetadata.ContainsKey("title"));
        Assert.Equal(title, capturedMetadata["title"]);
        _mockPlexApiService.Verify(x => x.UpdateMetadataAsync(ratingKey, It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithTitleAndLock_ShouldCallServiceWithLockedField()
    {
        // Arrange
        var ratingKey = "12345";
        var title = "New Movie Title";
        Dictionary<string, object>? capturedMetadata = null;

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Callback<string, Dictionary<string, object>>((key, metadata) => capturedMetadata = metadata)
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, title: title, lockTitle: true);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.NotNull(capturedMetadata);
        Assert.True(capturedMetadata.ContainsKey("title"));

        var titleField = capturedMetadata["title"] as Dictionary<string, object>;
        Assert.NotNull(titleField);
        Assert.Equal(title, titleField["value"]);
        Assert.Equal(true, titleField["locked"]);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithSummary_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var ratingKey = "12345";
        var summary = "This is a new summary for the movie";
        Dictionary<string, object>? capturedMetadata = null;

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Callback<string, Dictionary<string, object>>((key, metadata) => capturedMetadata = metadata)
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, summary: summary);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.Contains($"summary: '{summary[..Math.Min(50, summary.Length)]}'", response.Result);
        Assert.NotNull(capturedMetadata);
        Assert.True(capturedMetadata.ContainsKey("summary"));
        Assert.Equal(summary, capturedMetadata["summary"]);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithLongSummary_ShouldTruncateInResponseMessage()
    {
        // Arrange
        var ratingKey = "12345";
        var longSummary = new string('a', 100); // 100 character summary
        var expectedTruncated = longSummary[..50] + "...";

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, summary: longSummary);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.Contains($"summary: '{expectedTruncated}'", response.Result);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithContentRating_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var ratingKey = "12345";
        var contentRating = "PG-13";
        Dictionary<string, object>? capturedMetadata = null;

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Callback<string, Dictionary<string, object>>((key, metadata) => capturedMetadata = metadata)
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, contentRating: contentRating);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.Contains($"content rating: '{contentRating}'", response.Result);
        Assert.NotNull(capturedMetadata);
        Assert.True(capturedMetadata.ContainsKey("contentRating"));
        Assert.Equal(contentRating, capturedMetadata["contentRating"]);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithStudio_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var ratingKey = "12345";
        var studio = "Universal Studios";
        Dictionary<string, object>? capturedMetadata = null;

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Callback<string, Dictionary<string, object>>((key, metadata) => capturedMetadata = metadata)
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, studio: studio);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.Contains($"studio: '{studio}'", response.Result);
        Assert.NotNull(capturedMetadata);
        Assert.True(capturedMetadata.ContainsKey("studio"));
        Assert.Equal(studio, capturedMetadata["studio"]);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithYear_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var ratingKey = "12345";
        var year = 2024;
        Dictionary<string, object>? capturedMetadata = null;

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Callback<string, Dictionary<string, object>>((key, metadata) => capturedMetadata = metadata)
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, year: year);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.Contains($"year: {year}", response.Result);
        Assert.NotNull(capturedMetadata);
        Assert.True(capturedMetadata.ContainsKey("year"));
        Assert.Equal(year.ToString(), capturedMetadata["year"]);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithMultipleFields_ShouldCallServiceWithAllFields()
    {
        // Arrange
        var ratingKey = "12345";
        var title = "New Title";
        var summary = "New Summary";
        var rating = 8.5f;
        var contentRating = "R";
        var studio = "Warner Bros";
        var year = 2024;
        Dictionary<string, object>? capturedMetadata = null;

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Callback<string, Dictionary<string, object>>((key, metadata) => capturedMetadata = metadata)
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(
            ratingKey,
            title: title,
            summary: summary,
            rating: rating,
            contentRating: contentRating,
            studio: studio,
            year: year);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.Contains($"title: '{title}'", response.Result);
        Assert.Contains($"summary: '{summary}'", response.Result);
        Assert.Contains($"rating: {rating:F1}", response.Result);
        Assert.Contains($"content rating: '{contentRating}'", response.Result);
        Assert.Contains($"studio: '{studio}'", response.Result);
        Assert.Contains($"year: {year}", response.Result);

        Assert.NotNull(capturedMetadata);
        Assert.Equal(6, capturedMetadata.Count);
        Assert.Equal(title, capturedMetadata["title"]);
        Assert.Equal(summary, capturedMetadata["summary"]);
        Assert.Equal(rating.ToString("F1"), capturedMetadata["userRating"]);
        Assert.Equal(contentRating, capturedMetadata["contentRating"]);
        Assert.Equal(studio, capturedMetadata["studio"]);
        Assert.Equal(year.ToString(), capturedMetadata["year"]);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WithMultipleFieldsAndLocks_ShouldCallServiceWithCorrectLocks()
    {
        // Arrange
        var ratingKey = "12345";
        var title = "New Title";
        var summary = "New Summary";
        Dictionary<string, object>? capturedMetadata = null;

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .Callback<string, Dictionary<string, object>>((key, metadata) => capturedMetadata = metadata)
                          .Returns(Task.CompletedTask);

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(
            ratingKey,
            title: title,
            summary: summary,
            lockTitle: true,
            lockSummary: false);

        // Assert
        Assert.NotNull(response);
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.Contains("Metadata updated successfully", response.Result);
        Assert.NotNull(capturedMetadata);
        Assert.Equal(2, capturedMetadata.Count);

        // Title should be locked
        var titleField = capturedMetadata["title"] as Dictionary<string, object>;
        Assert.NotNull(titleField);
        Assert.Equal(title, titleField["value"]);
        Assert.Equal(true, titleField["locked"]);

        // Summary should not be locked (simple string value)
        Assert.Equal(summary, capturedMetadata["summary"]);
    }

    [Fact]
    public async Task UpdateMediaMetadataAsync_WhenServiceThrows_ShouldReturnErrorMessage()
    {
        // Arrange
        var ratingKey = "12345";
        var title = "New Title";
        var expectedError = "Test error message";

        _mockPlexApiService.Setup(x => x.UpdateMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                          .ThrowsAsync(new Exception(expectedError));

        // Act
        var response = await _mediaTools.UpdateMediaMetadataAsync(ratingKey, title: title);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Error);
        Assert.Contains("Test error message", response.Error.Message);
    }

}