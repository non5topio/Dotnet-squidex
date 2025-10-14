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
The test run failed due to two issues in `DefaultAppLogStoreTests.cs`:  
1. **Missing NuGet Package**: The `Moq` package is missing, causing a compilation error (`CS0246`).  
2. **Duplicate Using Directive**: A duplicate `using Squidex.Infrastructure.Log;` directive is present, causing a warning (`CS0105`).  

**Recommended Fixes:**  
1. **Add Moq Package**:  
   Run the following command in the test project directory:
   ```bash
   dotnet add package Moq
   ```
2. **Remove Duplicate Using Directive**:  
   Delete the duplicate `using Squidex.Infrastructure.Log;` line from the test file.

    [Fact]
    public async Task DeleteAppAsync_throws_when_app_is_null()
    {
        // Given
        var requestLogStore = new Mock<IRequestLogStore>();
        var sut = new DefaultAppLogStore(requestLogStore.Object);
    
        // When
        var act = () => sut.DeleteAppAsync(null, CancellationToken.None);
    
        // Then
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a missing `NSubstitute` NuGet package reference, which is required for mocking in `DefaultAppLogStoreTests.cs`. There is also a duplicate `using NodaTime;` directive, which is a warning and should be removed for clarity.

**Recommended Fixes:**  
1. Add the `NSubstitute` package to the test project:
   ```bash
   dotnet add package NSubstitute
   ```
2. Remove the duplicate `using NodaTime;` directive from `DefaultAppLogStoreTests.cs`.

    [Fact]
    public async Task ReadLogAsync_handles_cancellation_during_stream_writing()
    {
        // Given
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Now();
        var toTime = fromTime.Plus(Duration.FromSeconds(1));
        var stream = new MemoryStream();
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100); // Cancel after 100ms
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, cts.Token))
            .Returns(AsyncEnumerable.Empty<Request>());
    
        // When
        var act = async () => await sut.ReadLogAsync(appId, fromTime, toTime, stream, cts.Token);
    
        // Then
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a missing `NSubstitute` NuGet package reference, which is required for mocking in `DefaultAppLogStoreTests.cs`. There is also a duplicate `using NodaTime;` directive, which is a warning and should be removed.

**Recommended Fixes:**  
1. Add the `NSubstitute` package to the test project:
   ```bash
   dotnet add package NSubstitute
   ```
2. Remove the duplicate `using NodaTime;` directive from `DefaultAppLogStoreTests.cs`.

    [Fact]
    public async Task ReadLogAsync_handles_no_data()
    {
        // Given
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Now();
        var toTime = fromTime.Plus(Duration.FromSeconds(1));
        var stream = new MemoryStream();
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, CancellationToken.None))
            .Returns(AsyncEnumerable.Empty<Request>());
    
        // When
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, CancellationToken.None);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        Assert.StartsWith("Timestamp|RequestPath|RequestMethod|RequestElapsedMs|Costs|AuthClientId|AuthUserId|Bytes|CacheHits|CacheServer|CacheStatus|CacheTTL|StatusCode", content);
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a missing `NSubstitute` NuGet package reference, which is required for mocking in the test file. There is also a duplicate `using NodaTime;` directive, which is a warning but should be removed for clarity.

**Recommended Fixes:**
1. Add the `NSubstitute` package to the test project:
   ```bash
   dotnet add package NSubstitute
   ```
2. Remove the duplicate `using NodaTime;` directive from `DefaultAppLogStoreTests.cs`.

    [Fact]
    public async Task ReadLogAsync_handles_small_time_range()
    {
        // Given
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Now();
        var toTime = fromTime.Plus(Duration.FromMilliseconds(1));
        var stream = new MemoryStream();
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, CancellationToken.None))
            .Returns(AsyncEnumerable.Empty<Request>());
    
        // When
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, CancellationToken.None);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        Assert.StartsWith("Timestamp|RequestPath|RequestMethod|RequestElapsedMs|Costs|AuthClientId|AuthUserId|Bytes|CacheHits|CacheServer|CacheStatus|CacheTTL|StatusCode", content);
    }

*/
/*
FAILED TEST: The test run failed because the `Moq` namespace is missing, which is used for mocking in the test file. This typically means the:

1. **Missing NuGet Package**: The `Moq` NuGet package is not referenced in the test project.
2. **Namespace Conflict**: There is a duplicate `using NodaTime;` directive, which is a warning but may cause confusion.

### Recommended Fixes:

1. **Add Moq Package**:
   Run the following command in the test project directory:
   ```bash
   dotnet add package Moq
   ```

2. **Remove Duplicate Using Directive**:
   Remove the duplicate `using NodaTime;` line from the test file to clean up the code.

    [Fact]
    public async Task ReadLogAsync_throws_when_appId_is_null()
    {
        // Given
        var stream = new MemoryStream();
        var sut = new DefaultAppLogStore(new Mock<IRequestLogStore>().Object);
    
        // When
        var act = () => sut.ReadLogAsync(null, SystemClock.Now(), SystemClock.Now().Plus(Duration.FromSeconds(1)), stream);
    
        // Then
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

*/
/*
FAILED TEST: **Analysis:**  
The test compilation failed because the `NSubstitute` namespace is missing, which is required for mocking (`A.Fake<IRequestLogStore>()`). This indicates that the `NSubstitute` NuGet package is not referenced in the test project.

**Recommended Fix:**  
Add the `NSubstitute` NuGet package to the test project by running the following command in the project directory:
```bash
dotnet add package NSubstitute
```

    [Fact]
    public async Task LogAsync_handles_non_numeric_values()
    {
        // Given
        var appId = DomainId.NewGuid();
        var request = new RequestLog
        {
            UserClientId = "client123",
            UserId = "user456",
            Bytes = "invalid",
            CacheHits = "invalid",
            CacheServer = "server1",
            CacheStatus = "HIT",
            CacheTTL = "invalid",
            Costs = "invalid",
            ElapsedMs = "invalid",
            RequestMethod = "GET",
            RequestPath = "/api/content",
            StatusCode = "invalid",
            Timestamp = SystemClock.Now()
        };
    
        // When
        await sut.LogAsync(appId, request, CancellationToken.None);
    
        // Then
        A.CallTo(() => requestLogStore.LogAsync(A<Request>.Ignored, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

*/
/*
FAILED TEST: **Analysis:**  
The test compilation failed because the `NSubstitute` namespace is missing, which is required for mocking (`A.Fake<IRequestLogStore>()`). This indicates that the `NSubstitute` NuGet package is not referenced in the test project.

**Recommended Fix:**  
Add the `NSubstitute` NuGet package to the test project by running the following command in the project directory:
```bash
dotnet add package NSubstitute
```

    [Fact]
    public async Task LogAsync_handles_null_or_empty_string_values()
    {
        // Given
        var appId = DomainId.NewGuid();
        var request = new RequestLog
        {
            UserClientId = null,
            UserId = string.Empty,
            Bytes = null,
            CacheHits = string.Empty,
            CacheServer = null,
            CacheStatus = string.Empty,
            CacheTTL = null,
            ElapsedMs = "0",
            RequestMethod = "GET",
            RequestPath = "/api/content",
            StatusCode = "200",
            Timestamp = SystemClock.Now()
        };
    
        // When
        await sut.LogAsync(appId, request, CancellationToken.None);
    
        // Then
        A.CallTo(() => requestLogStore.LogAsync(A<Request>.Ignored, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

*/
/*
FAILED TEST: **Analysis:**  
The test compilation failed because the `NSubstitute` namespace is missing, which is used for mocking (`A.Fake<IRequestLogStore>()`). This typically means the `NSubstitute` NuGet package is not referenced in the test project.

**Recommended Fix:**  
Add the `NSubstitute` NuGet package to the test project (`Squidex.Domain.Apps.Entities.Tests.csproj`). Run the following command in the project directory:

```bash
dotnet add package NSubstitute
```

    [Fact]
    public async Task LogAsync_logs_valid_request_with_all_fields()
    {
        // Given
        var appId = DomainId.NewGuid();
        var request = new RequestLog
        {
            UserClientId = "client123",
            UserId = "user456",
            Bytes = "1024",
            CacheHits = "5",
            CacheServer = "server1",
            CacheStatus = "HIT",
            CacheTTL = "3600",
            Costs = "0.5",
            ElapsedMs = "150",
            RequestMethod = "GET",
            RequestPath = "/api/content",
            StatusCode = "200",
            Timestamp = SystemClock.Now()
        };
    
        // When
        await sut.LogAsync(appId, request, CancellationToken.None);
    
        // Then
        A.CallTo(() => requestLogStore.LogAsync(A<Request>.Ignored, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

*/
}
