using Xunit;
using IrysSDK.Tests.Fixtures;

namespace IrysSDK.Tests.Integration;

[Collection("SDK Integration Tests")]
public class NotificationTests
{
    private readonly SdkFixture _fixture;

    public NotificationTests(SdkFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should get user notifications")]
    public async Task GetNotifications_Should_Return_Notification_List()
    {
        // Act
        var response = await _fixture.Sdk.GetNotificationsAsync(limit: null, before: null, after: null);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Notifications);
        
        // If notifications exist, verify structure
        if (response.Notifications.Count > 0)
        {
            foreach (var notification in response.Notifications)
            {
                Assert.True(notification.Id > 0);
                // Type is a valid NotificationType enum
                Assert.True(Enum.IsDefined(typeof(NotificationType), notification.Type));
                Assert.True(notification.CreatedAt != default);
            }
        }
    }
}
