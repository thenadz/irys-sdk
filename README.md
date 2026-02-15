# Irys Mobile Client SDK

Unofficial .NET SDK for the Irys Mobile Client API, as used by the [311SA mobile app](https://www.sa.gov/Directory/Departments/311/Services/App).

Base URL:

https://irysteams.com

All endpoints are rooted under:

/api/mobile/client/

This SDK automatically manages session data between requests, storing and injected session token as needed throughout life of the session. The endpoints are defined based on observed behavior of the mobile app Feb 2026. Server behavior is subject to change at any time.

This library is built on an OpenAPI spec, which is used to generate a C# client by way of [NSwag](https://github.com/RicoSuter/NSwag).

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

``` csharp
var results = await sdk.SearchFlagsAsync(new FlagsSearchRequest
{
    Bounds = new GeoBounds
    {
        NorthEast = new GeoPoint { Latitude = 29.5, Longitude = -98.4 },
        SouthWest = new GeoPoint { Latitude = 29.3, Longitude = -98.6 }
    },
    Limit = 25
});
```

Limit range: 1--50

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
GET /avatar/{id}

## Organization

GET /org/details

## Device

POST /updatedevice

## Flags

POST /flags (multipart)\
POST /flags/search\
POST /flags/like\
GET /flags/categories

## Leaderboard

GET /leaderboard\
GET /leaderboard/me

------------------------------------------------------------------------

# Error Handling

All non-200 responses throw `ApiException`.

Exposes: - StatusCode - Response - Headers

------------------------------------------------------------------------

# License

Unofficial SDK. No affiliation with Irys.
