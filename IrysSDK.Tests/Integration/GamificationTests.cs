using Xunit;
using IrysSDK.Tests.Fixtures;

namespace IrysSDK.Tests.Integration;

[Collection("SDK Integration Tests")]
public class GamificationTests
{
    private readonly SdkFixture _fixture;

    public GamificationTests(SdkFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should get badge levels")]
    public async Task GetLevels_Should_Return_Badge_Levels()
    {
        // Act
        var response = await _fixture.Sdk.GetLevelsAsync(if_None_Match: null);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Levels);
        Assert.NotEmpty(response.Levels);
        
        // Verify each level has required fields
        foreach (var level in response.Levels)
        {
            Assert.True(level.Id > 0);
            Assert.False(string.IsNullOrEmpty(level.Badge_title));
        }
    }
}
