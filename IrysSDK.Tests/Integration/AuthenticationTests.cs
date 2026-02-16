using Xunit;
using IrysSDK.Tests.Fixtures;

namespace IrysSDK.Tests.Integration;

[Collection("SDK Integration Tests")]
public class AuthenticationTests
{
    private readonly SdkFixture _fixture;

    public AuthenticationTests(SdkFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should authenticate successfully with valid credentials")]
    public void Should_Be_Logged_In_After_Fixture_Initialization()
    {
        // Arrange & Act are done in fixture initialization
        
        // Assert
        Assert.True(_fixture.Sdk.IsLoggedIn);
        Assert.True(_fixture.AuthenticatedUserId > 0);
    }
}
