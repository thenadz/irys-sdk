using Xunit;
using IrysSDK;

namespace IrysSDK.Tests.Fixtures;

/// <summary>
/// Shared fixture for API integration tests.
/// Provides a single authenticated SDK instance reused across all test suites.
/// </summary>
public class SdkFixture : IAsyncLifetime
{
    /// <summary>Test user ID: Adriana Rocha Garcia (3,336 points)</summary>
    public const int TestUserId = 143477;

    public BaseSdk Sdk { get; set; } = default!;
    public int AuthenticatedUserId { get; set; }

    /// <summary>Load credentials from either environment or .env.local file</summary>
    private static (string, string) LoadEnv()
    {
        string email = string.Empty;
        string password = string.Empty;

        // Check if environment variables are already set (e.g., from GitHub Actions)
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IRYS_TEST_EMAIL")))
        {
            email = Environment.GetEnvironmentVariable("IRYS_TEST_EMAIL")!;
            password = Environment.GetEnvironmentVariable("IRYS_TEST_PASSWORD")!;
        }
        else
        {
          // Find .env.local in the solution root
          // From bin/Debug/net8.0 (4 directories deep), go up 4 levels to reach solution root
          var envFile = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env.local");
          envFile = Path.GetFullPath(envFile);

          if (File.Exists(envFile))
          {
            try
            {
                foreach (var line in File.ReadAllLines(envFile))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        if (key == "IRYS_TEST_EMAIL") email = value;
                        else if (key == "IRYS_TEST_PASSWORD") password = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FIXTURE] Warning: Could not load .env.local: {ex.Message}");
            }
          }
        }

        return (email, password);
    }

    /// <summary>Initialize SDK and authenticate before tests run</summary>
    public async Task InitializeAsync()
    {
        // Automatically load .env.local if environment variables aren't already set
        var (email, password) = LoadEnv();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            var missingVars = new List<string>();
            if (string.IsNullOrEmpty(email)) missingVars.Add("IRYS_TEST_EMAIL");
            if (string.IsNullOrEmpty(password)) missingVars.Add("IRYS_TEST_PASSWORD");
            
            throw new InvalidOperationException(
                $"FIXTURE INITIALIZATION FAILED: Missing credentials. Set environment variables or create .env.local file with: {string.Join(", ", missingVars)}");
        }
        
        try
        {
            Sdk = new BaseSdk();
            Console.WriteLine("[FIXTURE] Attempting login with test credentials...");
            
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };
            
            var loginResponse = await Sdk.LoginAsync(loginRequest);
            Console.WriteLine($"[FIXTURE] LoginResponse received");
            Console.WriteLine($"[FIXTURE] LoginResponse.User?.Id: {loginResponse?.User?.Id}");
            
            AuthenticatedUserId = loginResponse?.User?.Id ?? 0;
            Console.WriteLine($"[FIXTURE] AuthenticatedUserId set to: {AuthenticatedUserId}");
            Console.WriteLine($"[FIXTURE] IsLoggedIn: {Sdk.IsLoggedIn}");
            
            if (!Sdk.IsLoggedIn || AuthenticatedUserId <= 0)
            {
                throw new InvalidOperationException(
                    $"FIXTURE INITIALIZATION FAILED: Authentication failed. IsLoggedIn={Sdk.IsLoggedIn}, UserId={AuthenticatedUserId}");
            }
            
            Console.WriteLine("[FIXTURE] Authentication successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FIXTURE] INITIALIZATION ERROR: {ex.Message}");
            Console.WriteLine($"[FIXTURE] All tests will be skipped due to fixture initialization failure");
            throw;
        }
    }

    public Task DisposeAsync()
    {
        Sdk?.Dispose();
        return Task.CompletedTask;
    }
}

/// <summary>Collection definition for integration tests using shared SdkFixture</summary>
[CollectionDefinition("SDK Integration Tests")]
public class SdkIntegrationTestCollection : ICollectionFixture<SdkFixture>
{
    // This has no code, just used to define the collection
}
