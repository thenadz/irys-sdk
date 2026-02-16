using Xunit;
using IrysSDK.Tests.Fixtures;

namespace IrysSDK.Tests.Integration;

[Collection("SDK Integration Tests")]
public class GeocodingTests
{
    private readonly SdkFixture _fixture;

    public GeocodingTests(SdkFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should geocode coordinates to address")]
    public async Task GeocodeAddress_Should_Return_Address_String()
    {
        // Act
        var request = new GeocodeAddressRequest
        {
            Lat = 29.4263,
            Lng = -98.4936,
            Lang = "en"
        };
        var address = await _fixture.Sdk.GeocodeAddressAsync(request);

        // Assert
        Assert.NotNull(address);
        Assert.False(string.IsNullOrEmpty(address));
    }

    [Fact(DisplayName = "Should parse GIS coordinates")]
    public async Task ParseGIS_Should_Return_Location_Data()
    {
        // Act
        var request = new GISParseRequest
        {
            Lat = 29.4263,
            Long = -98.4936,
            Address = "106 E Houston St",
            LaganTypeKey = "graffiti_vof1"
        };
        var result = await _fixture.Sdk.ParseGISAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Location);
        // X and Y are strings representing coordinates
        Assert.False(string.IsNullOrEmpty(result.Location.X));
        Assert.False(string.IsNullOrEmpty(result.Location.Y));
    }
}
