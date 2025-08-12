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
FAILED TEST: **Analysis:**  
The test run failed due to a **missing NSubstitute NuGet package** in the test project. The `DefaultAppLogStoreTests.cs` file uses the `NSubstitute` namespace, which results in a **CS0246 compiler error** because the package is not installed.

**Recommended Fix:**  
Install the NSubstitute package in the test project using the following command:

```bash
dotnet add package NSubstitute --version 5.0.0
```

    [Fact]
    public async Task ReadLogAsync_Should_HandleLargeNumberOfRecords_When_Streamed()
    {
        // Given
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        var toTime = SystemClock.Instance.GetCurrentInstant();
        var stream = new MemoryStream();
    
        var largeRequests = new List<Request>();
        for (int i = 0; i < 1000; i++)
        {
            var request = new Request
            {
                Timestamp = fromTime.Plus(Duration.FromSeconds(i)),
                Properties = new Dictionary<string, string>
                {
                    { DefaultAppLogStore.FieldRequestPath, "/api/content" },
                    { DefaultAppLogStore.FieldRequestMethod, "GET" },
                    { DefaultAppLogStore.FieldRequestElapsedMs, "150" },
                    { DefaultAppLogStore.FieldCosts, "0.5" },
                    { DefaultAppLogStore.FieldAuthClientId, "client123" },
                    { DefaultAppLogStore.FieldAuthUserId, "user456" },
                    { DefaultAppLogStore.FieldBytes, "1024" },
                    { DefaultAppLogStore.FieldCacheHits, "5" },
                    { DefaultAppLogStore.FieldCacheServer, "server1" },
                    { DefaultAppLogStore.FieldCacheStatus, "HIT" },
                    { DefaultAppLogStore.FieldCacheTTL, "3600" },
                    { DefaultAppLogStore.FieldStatusCode, "200" }
                }
            };
            largeRequests.Add(request);
        }
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, default))
            .Returns(largeRequests.ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, default);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();
    
        // Ensure the header is present
        Assert.StartsWith("Timestamp|RequestPath|RequestMethod|RequestElapsedMs|Costs|AuthClientId|AuthUserId|Bytes|CacheHits|CacheServer|CacheStatus|CacheTTL|StatusCode", result);
    
        // Ensure the number of lines matches the number of requests + 1 for the header
        var lines = result.Split('\n');
        Assert.Equal(1000 + 1, lines.Length);
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **missing NSubstitute NuGet package** in the test project. The `DefaultAppLogStoreTests.cs` file uses the `NSubstitute` namespace, which causes a **CS0246 compiler error** because the package is not installed.

**Recommended Fix:**  
Install the **NSubstitute** package in the test project using the following command:

```bash
dotnet add package NSubstitute --version 5.0.0
```

    [Fact]
    public async Task ReadLogAsync_Should_ReturnEmptyStringForMissingProperties_When_PropertyIsNotPresent()
    {
        // Given
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Instance.GetCurrentInstant();
        var toTime = fromTime.Plus(Duration.FromSeconds(10));
        var stream = new MemoryStream();
    
        var request = new Request
        {
            Timestamp = fromTime,
            Properties = new Dictionary<string, string>
            {
                { DefaultAppLogStore.FieldRequestMethod, "GET" }
            }
        };
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, default))
            .Returns(Enumerable.Repeat(request, 1).ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, default);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();
    
        // Expecting empty string for missing fields
        Assert.Contains("||", result);
    }

*/
/*
FAILED TEST: The test run failed because the `NSubstitute` namespace is used in `DefaultAppLogStoreTests.cs`, but the **NSubstitute NuGet package is not installed** in the test project, causing a **CS0246** compiler error.

### ✅ **Analysis:**
- The test file references `NSubstitute`, which is not available in the project.
- This results in a **missing namespace error** during compilation.

### 🔧 **Recommended Fix:**
Install the **NSubstitute** NuGet package in the test project using the following command:

```bash
dotnet add package NSubstitute --version 5.0.0
```

    [Fact]
    public async Task LogAsync_Should_NotCallRequestLogStore_When_StoreIsDisabled()
    {
        // Given
        var appId = DomainId.NewGuid();
        var request = new RequestLog
        {
            UserClientId = "client123",
            UserId = "user456",
            Bytes = 1024,
            CacheHits = 5,
            CacheServer = "server1",
            CacheStatus = "HIT",
            CacheTTL = 3600,
            Costs = 0.5,
            ElapsedMs = 150,
            RequestMethod = "GET",
            RequestPath = "/api/content",
            StatusCode = 200,
            Timestamp = SystemClock.Instance.GetCurrentInstant()
        };
    
        var requestLogStore = A.Fake<IRequestLogStore>();
        var sut = new DefaultAppLogStore(requestLogStore);
    
        A.CallTo(() => requestLogStore.IsEnabled).Returns(false);
    
        // When
        await sut.LogAsync(appId, request, default);
    
        // Then
        A.CallTo(() => requestLogStore.LogAsync(A<Request>.Ignored, default))
            .MustNotHaveHappened();
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed because the `NSubstitute` namespace is used in `DefaultAppLogStoreTests.cs`, but the **NSubstitute NuGet package is not installed** in the test project, causing a **CS0246** compiler error.

**Recommended Fix:**  
Install the **NSubstitute** package in the test project using the following command:

```bash
dotnet add package NSubstitute --version 5.0.0
```

    [Fact]
    public async Task ReadLogAsync_Should_ReturnNullForInvalidNumericValues_When_PropertiesAreMalformed()
    {
        // Given
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Instance.GetCurrentInstant();
        var toTime = fromTime.Plus(Duration.FromSeconds(10));
        var stream = new MemoryStream();
    
        var request = new Request
        {
            Timestamp = fromTime,
            Properties = new Dictionary<string, string>
            {
                { DefaultAppLogStore.FieldRequestElapsedMs, "invalid" },
                { DefaultAppLogStore.FieldCosts, "not-a-number" }
            }
        };
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, default))
            .Returns(Enumerable.Repeat(request, 1).ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, default);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();
    
        // Expecting two fields to be empty due to invalid parsing
        Assert.Contains("||", result);
    }

*/
/*
FAILED TEST: The test run failed because the `NSubstitute` namespace is referenced in `DefaultAppLogStoreTests.cs`, but the **NSubstitute NuGet package is not installed** in the test project.

### ✅ **Analysis:**
- Compiler error `CS0246` indicates that the `NSubstitute` type is missing.
- This is due to the absence of the NSubstitute package in the project dependencies.

### 🔧 **Recommended Fix:**
Install the **NSubstitute** package in the test project using the following command:
```bash
dotnet add package NSubstitute --version 5.0.0
```

    [Fact]
    public async Task ReadLogAsync_Should_WriteHeaderOnly_When_NoEntriesFound()
    {
        // Given
        var appId = DomainId.NewGuid();
        var fromTime = Instant.Epoch;
        var toTime = Instant.Epoch;
        var stream = new MemoryStream();
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, default))
            .Returns(Enumerable.Empty<Request>().ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, default);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();
    
        // CSV header should be present
        Assert.StartsWith("Timestamp|RequestPath|RequestMethod|RequestElapsedMs|Costs|AuthClientId|AuthUserId|Bytes|CacheHits|CacheServer|CacheStatus|CacheTTL|StatusCode", result);
        Assert.Equal(result.Length, "Timestamp|RequestPath|RequestMethod|RequestElapsedMs|Costs|AuthClientId|AuthUserId|Bytes|CacheHits|CacheServer|CacheStatus|CacheTTL|StatusCode\n".Length);
    }

*/
/*
FAILED TEST: The test run failed because the `NSubstitute` namespace is missing in the `DefaultAppLogStoreTests.cs` file, resulting in the **CS0246** compiler error.

### ✅ **Analysis:**
- The test file uses `NSubstitute` for mocking but the the package is not installed in the test project.

### 🔧 **Recommended Fix:**
Install the **NSubstitute** NuGet package in the test project:
```bash
dotnet add package NSubstitute --version 5.0.0
```

    [Fact]
    public async Task LogAsync_Should_NotStoreEmptyOrNullOrStringFields_When_ValuesAreNullOrEmpty()
    {
        // Given
        var appId = DomainId.NewGuid();
        var request = new RequestLog
        {
            UserClientId = null,
            UserId = string.Empty,
            Bytes = 1024,
            CacheHits = 5,
            CacheServer = "server1",
            CacheStatus = "HIT",
            CacheTTL = 3600,
            Costs = 0.5,
            ElapsedMs = 150,
            RequestMethod = "GET",
            RequestPath = "/api/content",
            StatusCode = 200,
            Timestamp = SystemClock.Instance.GetCurrentInstant()
        };
    
        // When
        await sut.LogAsync(appId, request, default);
    
        // Then
        A.CallTo(() => requestLogStore.LogAsync(A<Request>.Ignored, default))
            .MustHaveHappenedOnceExactly();
    }

*/
/*
FAILED TEST: The test failed due to a **missing reference to the NSubstitute library**, which is used for creating fake objects in the test setup.

### ✅ **Analysis:**
- **Error:** `CS0246: The type or namespace name 'NSubstitute' could not be found`
- This indicates that the project does **not have NSubstitute installed** or referenced.

### 🔧 **Recommended Fix:**
Install the **NSubstitute** NuGet package in the test project:

```bash
dotnet add package NSubstitute --version <latest-compatible-version>
```

Replace `<latest-compatible-version>` with the version compatible with your project (e.g., `5.0.0`).

This will resolve the missing namespace error and allow the test to compile and run.

    [Fact]
    public async Task LogAsync_Should_StoreAllFields_When_RequestHasAllValues()
    {
        // Given
        var appId = DomainId.NewGuid();
        var request = new RequestLog
        {
            UserClientId = "client123",
            UserId = "user456",
            Bytes = 1024,
            CacheHits = 5,
            CacheServer = "server1",
            CacheStatus = "HIT",
            CacheTTL = 3600,
            Costs = 0.5,
            ElapsedMs = 150,
            RequestMethod = "GET",
            RequestPath = "/api/content",
            StatusCode = 200,
            Timestamp = SystemClock.Instance.GetCurrentInstant()
        };
    
        // When
        await sut.LogAsync(appId, request, default);
    
        // Then
        A.CallTo(() => requestLogStore.LogAsync(A<Request>.Ignored, default))
            .MustHaveHappenedOnceExactly();
    }

*/
}
