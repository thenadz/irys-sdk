# Irys Mobile Client SDK

Unofficial .NET SDK for the Irys Mobile Client API, as used by the [311SA mobile app](https://www.sa.gov/Directory/Departments/311/Services/App).

Base URL:

https://irysteams.com

All endpoints are rooted under:

/api/mobile/client/

This SDK automatically manages session data between requests, storing and injected session token as needed throughout life of the session. The endpoints are defined based on observed behavior of the mobile app Feb 2026. Server behavior is subject to change at any time.

This library is built on an OpenAPI spec, which is used to generate a C# client by way of [NSwag](https://github.com/RicoSuter/NSwag). This spec-first approach also enables generating other SDKs by way of the OpenAPI doc. NSwag supports TypeScript generation for frontend work and other libraries provide an even broader range of generation possiblities depending on your use case. New client libraries would simply need to replicate the manual logic to handle session tokens and other API-specific behavior, as demonstrated in `BaseSdk.cs`.

------------------------------------------------------------------------

# Authentication Model

The API uses two keys:

## 1. Static App Key

Sent on every request:
- Header: `appprivatekey`
- JSON body field: `private_key`

## 2. User Session Token

Returned from `LoginResponse.user_token`.

After login, SDK:
- Stores `user_token`
- Sends it as header: `authuser`
- Clears it after logout

All of this is handled automaticly in the middleware.

------------------------------------------------------------------------

# Common Workflows

## Login

``` csharp
var sdk = new BaseSdk();

var login = await sdk.LoginAsync(new LoginRequest
{
    Email = "user@example.com",
    Password = "password"
});
```

## Create Flag (Multipart Upload)

``` csharp
using var stream = File.OpenRead("photo.jpg");

var flag = await sdk.CreateFlagAsync(
    category_id: 1,
    description: "Graffiti on wall",
    latitude: 29.4241,
    longitude: -98.4936,
    image: new FileParameter(stream, "photo.jpg", "image/jpeg")
);
```

Latitude range: -90 to 90\
Longitude range: -180 to 180

## Search Flags

Geographic search uses a center point with radius:

``` csharp
var results = await sdk.SearchFlagsAsync(new FlagsSearchRequest
{
    Bounds = new { lat = 29.4239, long = -98.4936, radius = 5000 },
    Limit = 200,
    Status = new[] { 0, 1 }  // 0 = open, 1 = closed
});
```

**Geographic Search Details:**
- `lat`: Center latitude (-90 to 90)
- `long`: Center longitude (-180 to 180)  
- `radius`: Search radius in meters (all flags within radius returned)
- `limit`: Maximum results (1-200; API caps at 200 regardless)
- `status`: Array of status codes to filter (0=open, 1=closed)
- `categoryid`: Optional category filter
- `subcategoryid`: Optional subcategory filter

## Like Flag

``` csharp
await sdk.LikeFlagAsync(new LikeFlagRequest
{
    Flag_id = 123
});
```

## Leaderboard

``` csharp
var leaderboard = await sdk.GetLeaderboardAsync(
    fromdate: DateTimeOffset.UtcNow.AddDays(-7),
    todate: DateTimeOffset.UtcNow,
    limit: 20,
    if_None_Match: null
);
```

Date format: ISO 8601 date-time.

## Logout

``` csharp
await sdk.LogoutAsync(new BaseAppRequest());
```

------------------------------------------------------------------------

# Endpoint Summary

## Auth

POST /login\
POST /logout\
POST /signup\
POST /signup/email/confirmcode

## User

GET /me\
GET /{userId}\
GET /{userId}/flags\
GET /{userId}/stats/totals\
GET /{userId}/followers\
GET /{userId}/following\
GET /{userId}/follows/{otherId}\
GET /avatar/{id}

## Organization

GET /org/details

## Device

POST /updatedevice

## Flags

POST /flags (multipart)\
POST /flags/search\
POST /flags/like\
GET /flags/categories\
GET /flags/categories/{categoryId}/subcategories\
GET /flags/details/{flagId}

## Leaderboard

GET /leaderboard\
GET /leaderboard/me

## Gamification

GET /levels

## Notifications

GET /notifications

## Geocoding & GIS

POST /geocode/address\
POST /gis/parse

------------------------------------------------------------------------

# Additional Endpoints

## User Profile

Get another user's profile:
``` csharp
var user = await sdk.GetUserAsync(userId: 123);
```

Get user's own flags:
``` csharp
var flags = await sdk.GetUserFlagsAsync(userId: 139922, limit: 100);
```

Get user statistics:
``` csharp
var stats = await sdk.GetUserStatsTotalsAsync(userId: 139922);
// Returns: { result: 1, totals: { flags: 1, followers: 0, following: 0, points: 6 } }
```

## Social Features

Get followers:
``` csharp
var followers = await sdk.GetUserFollowersAsync(userId: 123, page: 0, limit: 10);
```

Get following list:
``` csharp
var following = await sdk.GetUserFollowingAsync(userId: 123, page: 0, limit: 50);
```

Check if following user:
``` csharp
var isFollowing = await sdk.CheckFollowsAsync(userId: 123, otherId: 456);
```

## Flag Details

Get detailed flag information:
``` csharp
var details = await sdk.GetFlagDetailsAsync(flagId: "507f1f77bcf86cd799439011");
```

Get category subcategories:
``` csharp
var subcats = await sdk.GetSubcategoriesAsync(categoryId: 124);
```

## Gamification

Get user levels:
``` csharp
var levels = await sdk.GetLevelsAsync();
// Returns: { result: 1, msg: "", err_code: "", levels: [{id:21, badge_title:"Volunteer", points_earned:100, icon_url:"..."}, ...] }
```

Get notifications:
``` csharp
var notifications = await sdk.GetNotificationsAsync(limit: 50);
```

## Geocoding & GIS

Reverse geocode coordinates to address string:
``` csharp
var geocoded = await sdk.GeocodeAddressAsync(new GeocodeAddressRequest
{
    Lat = 29.4263,
    Lng = -98.4936
});
// Returns: plain string "106 E Houston St, San Antonio, TX 78205, USA"
```

Parse GIS data (convert lat/long to State Plane coordinates):
``` csharp
var gis = await sdk.ParseGISAsync(new GISParseRequest
{
    Lat = 29.4263,
    Long = -98.4936,
    Address = "106 E Houston St",
    LaganTypeKey = "sidewalk_investigation"
});
// Returns: { result: 0, msg: "", location: {x: string, y: string}, location_attributes: {...}, geometry: {...}, locationType: "...", err_code: "" }
```

------------------------------------------------------------------------

# Error Handling

All non-200 responses throw `ApiException`.

Exposes:
- StatusCode
- Response
- Headers

------------------------------------------------------------------------

# License

Unofficial SDK. No affiliation with Irys.
