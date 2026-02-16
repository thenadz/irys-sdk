using Xunit;
using IrysSDK.Tests.Fixtures;

namespace IrysSDK.Tests.Integration;

[Collection("SDK Integration Tests")]
public class SocialTests
{
    private readonly SdkFixture _fixture;

    public SocialTests(SdkFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should get user followers")]
    public async Task GetFollowers_Should_Return_User_List()
    {
        // Act
        var response = await _fixture.Sdk.GetUserFollowersAsync(SdkFixture.TestUserId, null, null);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Total_count >= 0);
        
        // If followers exist, verify structure (Users might be null if none)
        if (response.Users != null && response.Users.Count > 0)
        {
            foreach (var user in response.Users)
            {
                Assert.True(user.User_id > 0);
                Assert.False(string.IsNullOrEmpty(user.Name));
            }
        }
    }

    [Fact(DisplayName = "Should get users being followed")]
    public async Task GetFollowing_Should_Return_User_List()
    {
        // Act
        var response = await _fixture.Sdk.GetUserFollowingAsync(SdkFixture.TestUserId, null, null);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Total_count >= 0);
        
        // If following exists, verify structure (Users might be null if none)
        if (response.Users != null && response.Users.Count > 0)
        {
            foreach (var user in response.Users)
            {
                Assert.True(user.User_id > 0);
                Assert.False(string.IsNullOrEmpty(user.Name));
            }
        }
    }
}
