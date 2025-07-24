// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using NodaTime;
using Squidex.Domain.Apps.Entities.TestHelpers;
using Squidex.Infrastructure.Log;

namespace Squidex.Domain.Apps.Entities.Apps;

public class DefaultAppLogStoreTests : GivenContext
{
    private readonly IRequestLogStore requestLogStore = A.Fake<IRequestLogStore>();
    private readonly DefaultAppLogStore sut;

    public DefaultAppLogStoreTests()
    {
        sut = new DefaultAppLogStore(requestLogStore);
    }

/*
FAILED TEST: **Test Run Failure Analysis:**

- **Missing NSubstitute Package:** The test file uses `NSubstitute` but the package is not installed, causing `CS0246` errors.
- **Incorrect NodaTime Usage:** `Instant.Epoch` is not a valid API; should use `Instant.FromUnixTimeSeconds(0)`.
- **Missing Using Directive:** `Squidex.Domain.Apps.Core.Apps` is missing, causing `DomainId` to be unresolved.
- **Testing Internal Method `Append`:** The test directly calls the private `Append` method, which is not accessible.
- **StyleCop Violations:** Using directive order and trailing whitespace issues are present.

**Recommended Fixes:**

1. **Install NSubstitute:**
   ```bash
   dotnet add package NSubstitute
   ```

2. **Replace `Instant.Epoch` with:**
   ```csharp
   Instant.FromUnixTimeSeconds(0)
   ```

3. **Add Missing Using Directive:**
   ```csharp
   using Squidex.Domain.Apps.Core.Apps;
   ```

4. **Refactor Tests to Use Public API:**
   - Replace direct calls to `Append` with usage of the public `LogAsync` method.

5. **Fix Code Style Issues:**
   - Reorder using directives alphabetically.
   - Remove trailing whitespace.

    [Fact]
    public async Task ReadLogAsync_handles_large_number_of_entries()
    {
        // Given
        var appId = DomainId.NewGuid();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        var requests = new List<Request>();
    
        for (int i = 0; i < 1000; i++)
        {
            requests.Add(new Request
            {
                Properties = new Dictionary<string, string>
                {
                    { "RequestPath", "/api" },
                    { "RequestMethod", "GET" },
                    { "RequestElapsedMs", "100" },
                    { "Costs", "0.5" },
                    { "AuthClientId", "client" },
                    { "AuthUserId", "user" },
                    { "Bytes", "1024" },
                    { "CacheHits", "10" },
                    { "CacheServer", "server" },
                    { "CacheStatus", "hit" },
                    { "CacheTTL", "3600" },
                    { "StatusCode", "200" }
                },
                Timestamp = SystemClock.Instance.GetCurrentInstant()
            });
        }
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), A<Instant>._, A<Instant>._, A<CancellationToken>._)).Returns(requests.ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, Instant.Min, Instant.Max, stream, CancellationToken.None);
    
        // Then
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();
    
        Assert.Contains("Timestamp|RequestPath|RequestMethod|RequestElapsedMs|Costs|AuthClientId|AuthUserId|Bytes|CacheHits|CacheServer|CacheStatus|CacheTTL|StatusCode", result);
        Assert.Equal(1001, result.Split('\n').Length);
    }

*/
/*
FAILED TEST: **Test Run Failure Analysis:**

- The test file `DefaultAppLogStoreTests.cs` is missing the `NSubstitute` NuGet package, causing compilation errors (`CS0246`).
- The `Append` method is being tested directly, but it is not publicly accessible, leading to test failures.
- The test is using `Instant.Epoch`, which is not a valid `NodaTime` API.
- Missing `using` directive for `Squidex.Domain.Apps.Core.Apps` (needed for `DomainId`).
- Code style issues like incorrect using directive order and trailing whitespace are present.

**Recommended Fixes:**

1. **Install NSubstitute:**
   ```bash
   dotnet add package NSubstitute
   ```

2. **Refactor tests to use the public `LogAsync` method instead of internal `Append`.**

3. **Replace `Instant.Epoch` with `Instant.FromUnixTimeSeconds(0)`.**

4. **Add missing using directive:**
   ```csharp
   using Squidex.Domain.Apps.Core.Apps;
   ```

5. **Fix using directive order and remove trailing whitespace to comply with StyleCop.**

    [Fact]
    public async Task ReadLogAsync_writes_header_but_no_data_for_no_entries()
    {
        // Given
        var appId = DomainId.NewGuid();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), A<Instant>._, A<Instant>._, A<CancellationToken>._)).Returns(Enumerable.Empty<Request>().ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, Instant.Epoch, Instant.Epoch, stream, CancellationToken.None);
    
        // Then
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();
    
        Assert.Contains("Timestamp|RequestPath|RequestMethod|RequestElapsedMs|Costs|AuthClientId|AuthUserId|Bytes|CacheHits|CacheServer|CacheStatus|CacheTTL|StatusCode", result);
        Assert.Equal(1, result.Split('\n').Length);
    }

*/
/*
FAILED TEST: **Test Run Failure Analysis:**

1. **Missing `DomainId` Type:**
   - **Fix:** Add `using Squidex.Domain.Apps.Core.Apps;` to the test file.

2. **Incorrect Usage of `Instant.Epoch`:**
   - **Fix:** Replace `Instant.Epoch` with `Instant.FromUnixTimeSeconds(0)`.

3. **Missing `NSubstitute` Dependency:**
   - **Fix:** Add `NSubstitute` to the project:
     ```bash
     dotnet add package NSubstitute
     ```

4. **Testing Internal Method `Append`:**
   - **Fix:** Refactor tests to use public `LogAsync` method instead of internal `Append`.

5. **Missing `GetDouble` and `GetLong` Methods:**
   - **Fix:** Ensure tests are calling methods on the `Request` object, not the store. Use the internal methods via reflection or make them internal with `InternalsVisibleTo`.

6. **StyleCop Warnings:**
   - **Fix:** Fix using directive order and remove trailing whitespace.

    [Fact]
    public void GetDouble_and_GetLong_return_null_for_invalid_values()
    {
        // Given
        var request = new Request
        {
            Properties = new Dictionary<string, string>
            {
                { "Field1", "abc" },
                { "Field2", "123.45" },
                { "Field3", "invalid" }
            }
        };
    
        // When / Then
        Assert.Null(sut.GetDouble(request, "Field1"));
        Assert.Equal(123.45, sut.GetDouble(request, "Field2"));
        Assert.Null(sut.GetDouble(request, "Field3"));
    
        Assert.Null(sut.GetLong(request, "Field1"));
        Assert.Equal(123, sut.GetLong(request, "Field2"));
        Assert.Null(sut.GetLong(request, "Field3"));
    }

*/
/*
FAILED TEST: **Test Run Failure Analysis:**

1. **Missing `DomainId` Type:**
   - **Cause:** The test file is missing a reference to the `DomainId` type.
   - **Fix:** Add the appropriate `using` directive for `Squidex.Domain.Apps.Core.Apps` at the top of the test file.

2. **Incorrect Usage of `Instant.Epoch`:**
   - **Cause:** The `Instant` class from `NodaTime` does not have a static `Epoch` property.
   - **Fix:** Replace `Instant.Epoch` with `Instant.FromUnixTimeSeconds(0)` or use the correct `NodaTime` API for epoch time.

3. **Missing `NSubstitute` Dependency:**
   - **Cause:** The test uses `NSubstitute` but the package is not referenced.
   - **Fix:** Add the `NSubstitute` NuGet package to the test project:
     ```bash
     dotnet add package NSubstitute
     ```

4. **Testing Internal Method `Append`:**
   - **Cause:** The test is directly calling the internal `Append` method, which is not accessible.
   - **Fix:** Refactor the test to verify behavior through the public `LogAsync` method, not internal implementation details.

5. **StyleCop Warnings:**
   - **Cause:** Minor code style issues like incorrect using directive order and trailing whitespace.
   - **Fix:** Fix the order of `using` directives and remove trailing whitespace to comply with StyleCop rules.

    [Fact]
    public void ReadLogAsync_throws_exception_when_appId_is_null()
    {
        // Given
        DomainId appId = null;
    
        // When / Then
        Assert.Throws<ArgumentNullException>(() => sut.ReadLogAsync(appId, Instant.Epoch, Instant.Epoch, new MemoryStream(), CancellationToken.None));
    }

*/
/*
FAILED TEST: The test run failed because the test file `DefaultAppLogStoreTests.cs` is calling a method `Append` with three arguments, but the `DefaultAppLogStore` class does not expose such an overload in its public API. The `Append` method is private and internal to the class, making it inaccessible to the test.

**Analysis:**
- The test is attempting to verify the behavior of the `Append` method directly, which is not exposed.
- This leads to compilation errors (`CS1501: No overload for method 'Append' takes 3 arguments`).

**Recommended Fix:**
Refactor the test to verify the behavior indirectly by inspecting the final state or output of the `LogAsync` method, such as checking the properties of the request object after logging, rather than testing the internal `Append` method directly.

    [Fact]
    public void Append_methods_ignore_null_or_empty_values()
    {
        // Given
        var request = new Request();
    
        // When
        sut.Append(request, "Field1", null);
        sut.Append(request, "Field2", string.Empty);
        sut.Append(request, "Field3", "   ");
        sut.Append(request, "Field4", 123);
    
        // Then
        Assert.False(request.Properties.ContainsKey("Field1"));
        Assert.False(request.Properties.ContainsKey("Field2"));
        Assert.False(request.Properties.ContainsKey("Field3"));
        Assert.Equal("123", request.Properties["Field4"]);
    }

*/
/*
FAILED TEST: **Analysis:**

The test failed because the compiler could not find the `NSubstitute` namespace, which is used in the test file (`DefaultAppLogStoreTests.cs`). This typically happens when:

- The `NSubstitute` NuGet package is not installed.
- The project file is missing a reference to `NSubstitute`.

**Recommended Fix:**

Add the `NSubstitute` NuGet package to the test project (`Squidex.Domain.Apps.Entities.Tests.csproj`) by running the following command:

```bash
dotnet add package NSubstitute
```

Or manually edit the `.csproj` file to include:

```xml
<PackageReference Include="NSubstitute" Version="5.0.0" />
```

Ensure the version matches the one compatible with your test framework and .NET version.

    [Fact]
    public async Task LogAsync_returns_completed_task_when_logging_is_disabled()
    {
        // Given
        A.CallTo(() => requestLogStore.IsEnabled).Returns(false);
        var request = new RequestLog
        {
            UserClientId = "client",
            UserId = "user",
            Bytes = "1024",
            Costs = 0.5,
            CacheHits = "10",
            CacheServer = "server",
            CacheStatus = "hit",
            CacheTTL = "3600",
            RequestElapsedMs = 100,
            RequestMethod = "GET",
            RequestPath = "/api",
            StatusCode = "200",
            Timestamp = SystemClock.Instance.GetCurrentInstant()
        };
    
        // When
        await sut.LogAsync(DomainId.NewGuid(), request, CancellationToken.None);
    
        // Then
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

*/
}
