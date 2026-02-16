using Xunit;
using IrysSDK.Tests.Fixtures;
using IrysSDK;

namespace IrysSDK.Tests.Integration;

[Collection("SDK Integration Tests")]
public class FlagTests
{
    private readonly SdkFixture _fixture;

    public FlagTests(SdkFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should get flag categories")]
    public async Task GetFlagCategories_Should_Return_Categories()
    {
        // Act
        var response = await _fixture.Sdk.GetFlagCategoriesAsync(null);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Categories);
        Assert.NotEmpty(response.Categories);
        
        // Verify each category has required fields
        foreach (var category in response.Categories)
        {
            Assert.True(category.Id > 0);
            Assert.False(string.IsNullOrEmpty(category.Name));
        }
    }

    [Fact(DisplayName = "Should get flag subcategories")]
    public async Task GetSubcategories_Should_Return_Subcategories_With_LaganTypeKey()
    {
        // Act - Use category 130 which has rich survey data (verified in HAR)
        var response = await _fixture.Sdk.GetSubcategoriesAsync(130, null);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Subcategories);
        Assert.NotEmpty(response.Subcategories);
        
        // Verify each subcategory has required fields
        foreach (var subcategory in response.Subcategories)
        {
            Assert.True(subcategory.Id > 0);
            Assert.False(string.IsNullOrEmpty(subcategory.Name));
            // Survey data should be present for category 130
            Assert.NotNull(subcategory.Survey_title);
            Assert.True(subcategory.Surveys_id > 0);
        }
    }

    [Fact(DisplayName = "Should search flags with pagination")]
    public async Task SearchFlags_Should_Return_Flags_With_Pagination()
    {
        // Act
        var request = new FlagsSearchRequest
        {
            Limit = 10,
            Objextraprops = new FlagsSearchObjExtraProps
            {
                Lagan_detailsPriv = false
            }
        };
        var response = await _fixture.Sdk.SearchFlagsAsync(request);

        // Assert
        Assert.NotNull(response);
        
        // If flags exist, verify structure (response.Flags might be null if no results)
        if (response.Flags != null && response.Flags.Count > 0)
        {
            Assert.True(response.Flags.Count <= 10,
                $"Expected at most 10 flags, got {response.Flags.Count}");
            
            // Verify each flag has required fields
            foreach (var flag in response.Flags)
            {
                Assert.True(flag.Folio > 0);
                Assert.False(string.IsNullOrEmpty(flag.Description));
            }
        }
    }
}
