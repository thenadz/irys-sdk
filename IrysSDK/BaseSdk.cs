namespace IrysSDK;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

public class BaseSdk : InternalBaseSdk
{
  public override async Task<LoginResponse> LoginAsync(LoginRequest body, CancellationToken cancellationToken)
  {
    // clear session token before attempting login
    UserPrivateKey = null;

    // do login
    LoginResponse resp = await base.LoginAsync(body, cancellationToken);

    // intercept session token for use in subsequent requests
    if (!string.IsNullOrWhiteSpace(resp.User_token))
    {
      UserPrivateKey = resp.User_token;
    }

    return resp;
  }

  public override async Task LogoutAsync(BaseAppRequest body, CancellationToken cancellationToken)
  {
    // do logout
    await base.LogoutAsync(body, cancellationToken);

    // If we got here, logout succeeded (base throws on non-200)
    UserPrivateKey = null;
  }
}

public partial class InternalBaseSdk : IDisposable
{
  // Magic app static value as of Feb 2026 - likely changes with app version
  internal const string AppPrivateKey = "thIJbZlnPPHyuGdewVFhuzguJVeodCrdURWMoFlq";

  // Static setup logic for serialization
  static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
  {
    // Must be in both headers and the body - below injects into both
    DefaultJsonTypeInfoResolver resolver = new DefaultJsonTypeInfoResolver();
    resolver.Modifiers.Add(typeInfo =>
    {
      if (typeof(BaseAppRequest).IsAssignableFrom(typeInfo.Type))
      {
        typeInfo.OnSerializing = req => ((BaseAppRequest)req).Private_key = AppPrivateKey;
      }
    });
    settings.TypeInfoResolver = resolver;
  }

  // store session from login for all authenticated requests
  // NOTE: Generally the SDK will populate for you, but in test harness and other
  // instances it can be manually populated to avoid needing login call if you already
  // have a valid session token
  public string? UserPrivateKey { get; set; }

  // helper to check if we have a session token (not a guarantee of validity, but better than nothing)
  public bool IsLoggedIn => !string.IsNullOrWhiteSpace(UserPrivateKey);

  private static HttpClient BuildHttpClient()
  {
    return new HttpClient(new HttpClientHandler
    {
        AutomaticDecompression = System.Net.DecompressionMethods.GZip |
                                 System.Net.DecompressionMethods.Deflate |
                                 System.Net.DecompressionMethods.Brotli
    });
  }

  public InternalBaseSdk() : this(BuildHttpClient())
  {
  }

  // Instance setup logic for serialization and transport
  partial void Initialize()
  {
    // setup headers to be sent with each request
    HttpRequestHeaders reqHdrs = _httpClient.DefaultRequestHeaders;
    reqHdrs.Add("appprivatekey", AppPrivateKey);

    // Accept values
    HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept = reqHdrs.Accept;
    accept.Clear();
    accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
    accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

    // language
    HttpHeaderValueCollection<StringWithQualityHeaderValue> language = reqHdrs.AcceptLanguage;
    language.Clear();

    language.Add(new StringWithQualityHeaderValue("en-US"));
    language.Add(new StringWithQualityHeaderValue("en", 0.9));

    // user agent
    HttpHeaderValueCollection<ProductInfoHeaderValue> userAgnt = reqHdrs.UserAgent;
    userAgnt.Clear();
    userAgnt.ParseAdd("311SA/73 CFNetwork/3860.300.31 Darwin/25.2.0");

    // priority
    reqHdrs.Add("Priority", "u=3, i");
  }

  partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
  {
    if (!string.IsNullOrWhiteSpace(UserPrivateKey))
    {
      request.Headers.Add("authuser", UserPrivateKey);
    }
  }

  public void Dispose()
  {
    _httpClient.Dispose();
  }
}
