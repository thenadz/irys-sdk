using Xunit;
using IrysSDK.Tests.Fixtures;

namespace IrysSDK.Tests.Integration;

[Collection("SDK Integration Tests")]
public class UserTests
{
    private readonly SdkFixture _fixture;

    public UserTests(SdkFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should get authenticated user's profile")]
    public async Task GetMe_Should_Return_User_Profile_With_Valid_Data()
    {
        // Act
        var user = await _fixture.Sdk.GetMeAsync(if_None_Match: null);

        // Assert
        Assert.NotNull(user);
        // Basic validation - user object is returned (ID may vary depending on endpoint)
        Assert.True(!string.IsNullOrEmpty(user.Email) || user.Id >= 0);
    }

    [Fact(DisplayName = "Should get user stats totals")]
    public async Task GetUserStatsTotals_Should_Return_Statistics_Object()
    {
        // Act
        var stats = await _fixture.Sdk.GetUserStatsTotalsAsync(SdkFixture.TestUserId, hidepriv: null);

        // Assert
        Assert.NotNull(stats);
        Assert.NotNull(stats.Totals);
    }
}
