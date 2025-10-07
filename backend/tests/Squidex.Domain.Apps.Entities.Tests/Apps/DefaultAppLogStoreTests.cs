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
using System.Globalization;
using FakeItEasy;
using NodaTime;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Log;
using Xunit;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NodaTime;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Log;
using Xunit;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NodaTime;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Log;
using Xunit;
using Xunit;
using FakeItEasy;
using NodaTime;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Log;
using Squidex.Domain.Apps.Entities.Apps;
using Xunit;
using FakeItEasy;
using NodaTime;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Log;
using System.Text;
using Xunit;
using FakeItEasy;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Log;
using NodaTime;

namespace Squidex.Domain.Apps.Entities.Apps;

public class DefaultAppLogStoreTests : GivenContext
{
    private readonly IRequestLogStore requestLogStore = A.Fake<IRequestLogStore>();
    private readonly DefaultAppLogStore sut;

    public DefaultAppLogStoreTests()
    {
        sut = new DefaultAppLogStore(requestLogStore);
    }

    [Fact]
    public async Task Should_format_timestamps_and_numbers_culture_invariant()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = Instant.FromUnixTimeSeconds(1000);
        var toTime = Instant.FromUnixTimeSeconds(2000);
        var stream = new MemoryStream();
        var timestamp = Instant.FromUnixTimeSeconds(1500);
        
        var request = new Request
        {
            Key = appId.ToString(),
            Timestamp = timestamp,
            Properties = new Dictionary<string, string>
            {
                ["RequestPath"] = "/api/test",
                ["RequestElapsedMs"] = "123.45",
                ["Costs"] = "0.5",
                ["Bytes"] = "1024",
                ["StatusCode"] = "200",
            },
        };
        
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, A<CancellationToken>._))
            .Returns(new[] { request }.ToAsyncEnumerable());
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream);
    
        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.Contains(timestamp.ToString(), lines[1], StringComparison.Ordinal);
        Assert.Contains("123.45", lines[1], StringComparison.Ordinal);
        Assert.Contains("0.5", lines[1], StringComparison.Ordinal);
        Assert.Contains("1024", lines[1], StringComparison.Ordinal);
    }


    [Fact]
    public async Task Should_handle_large_number_of_log_entries()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = Instant.FromUnixTimeSeconds(1000);
        var toTime = Instant.FromUnixTimeSeconds(10000);
        var stream = new MemoryStream();
        
        var requests = Enumerable.Range(0, 1000).Select(i => new Request
        {
            Key = appId.ToString(),
            Timestamp = Instant.FromUnixTimeSeconds(1000 + i),
            Properties = new Dictionary<string, string>
            {
                ["RequestPath"] = $"/api/test/{i}",
                ["RequestMethod"] = "GET",
                ["StatusCode"] = "200",
            },
        });
        
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, A<CancellationToken>._))
            .Returns(requests.ToAsyncEnumerable());
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream);
    
        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(1001, lines.Length);
        Assert.Contains("Timestamp", lines[0], StringComparison.Ordinal);
        Assert.Contains("/api/test/0", lines[1], StringComparison.Ordinal);
        Assert.Contains("/api/test/999", lines[1000], StringComparison.Ordinal);
    }


    [Fact]
    public async Task Should_log_request_with_boundary_numeric_values()
    {
        // Arrange
        A.CallTo(() => requestLogStore.IsEnabled).Returns(true);
        
        var appId = DomainId.NewGuid();
        var timestamp = SystemClock.Instance.GetCurrentInstant();
        var requestLog = new RequestLog
        {
            Bytes = long.MaxValue,
            Costs = double.MaxValue,
            ElapsedMs = long.MinValue,
            StatusCode = int.MaxValue,
            CacheHits = long.MaxValue,
            CacheTTL = int.MaxValue,
            Timestamp = timestamp,
        };
    
        Request? capturedRequest = null;
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes((Request r, CancellationToken ct) => capturedRequest = r)
            .Returns(Task.CompletedTask);
    
        // Act
        await sut.LogAsync(appId, requestLog);
    
        // Assert
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        
        Assert.NotNull(capturedRequest);
        Assert.Equal(long.MaxValue.ToString(CultureInfo.InvariantCulture), capturedRequest.Properties["Bytes"]);
        Assert.Equal(double.MaxValue.ToString(CultureInfo.InvariantCulture), capturedRequest.Properties["Costs"]);
        Assert.Equal(long.MinValue.ToString(CultureInfo.InvariantCulture), capturedRequest.Properties["RequestElapsedMs"]);
        Assert.Equal(int.MaxValue.ToString(CultureInfo.InvariantCulture), capturedRequest.Properties["StatusCode"]);
        Assert.Equal(long.MaxValue.ToString(CultureInfo.InvariantCulture), capturedRequest.Properties["CacheHits"]);
        Assert.Equal(int.MaxValue.ToString(CultureInfo.InvariantCulture), capturedRequest.Properties["CacheTTL"]);
    }


    [Fact]
    public async Task Should_preserve_special_characters_in_string_fields()
    {
        // Arrange
        A.CallTo(() => requestLogStore.IsEnabled).Returns(true);
        
        var appId = DomainId.NewGuid();
        var timestamp = SystemClock.Instance.GetCurrentInstant();
        var requestLog = new RequestLog
        {
            UserClientId = "client|123",
            RequestPath = "api/test|data",
            CacheServer = "cache\nserver",
            RequestMethod = "POST",
            Timestamp = timestamp
        };
    
        Request? capturedRequest = null;
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes((Request r, CancellationToken ct) => capturedRequest = r)
            .Returns(Task.CompletedTask);
    
        // Act
        await sut.LogAsync(appId, requestLog);
    
        // Assert
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        
        Assert.NotNull(capturedRequest);
        Assert.Equal("client|123", capturedRequest.Properties["AuthClientId"]);
        Assert.Equal("api/test|data", capturedRequest.Properties["RequestPath"]);
        Assert.Equal("cache\nserver", capturedRequest.Properties["CacheServer"]);
    }


    [Fact]
    public async Task Should_handle_cancellation_during_enumeration()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = Instant.FromUnixTimeSeconds(1000);
        var toTime = Instant.FromUnixTimeSeconds(2000);
        var stream = new MemoryStream();
        var cts = new CancellationTokenSource();
        
        var requests = new List<Request>
        {
            new Request
            {
                Key = appId.ToString(),
                Timestamp = Instant.FromUnixTimeSeconds(1500),
                Properties = new Dictionary<string, string> { ["RequestPath"] = "/api/test1" }
            }
        };
        
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, A<CancellationToken>._))
            .Returns(CancelAfterFirstAsync(requests, cts));
    
        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            sut.ReadLogAsync(appId, fromTime, toTime, stream, cts.Token));
    }
    
    private static async IAsyncEnumerable<Request> CancelAfterFirstAsync(
        IEnumerable<Request> requests,
        CancellationTokenSource cts,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var request in requests)
        {
            yield return request;
            cts.Cancel();
            await Task.Delay(10, CancellationToken.None);
        }
    }

/*
FAILED TEST: ## Analysis

The test compilation **failed** due to **1 critical error** and **multiple code style warnings**.

### Critical Error

**Line 82**: `await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ReadLogAsync(null, ...))` - Cannot convert `null` to non-nullable `DomainId` struct. The `ReadLogAsync` method expects a `DomainId` parameter which is a value type and cannot be null.

### Code Style Warnings

- **Multiple duplicate using directives** throughout the file (lines 14, 16, 24, 26, 34-36, 40-43, 46, 48-50, 54-56)
- **Using directives not alphabetically ordered** - System namespaces should appear before third-party and project namespaces

## Recommended Fixes

1. **Fix the null argument test** (Line 82):
   - Use `default(DomainId)` instead of `null`, OR
   - Remove this test entirely if testing null validation isn't applicable for value types

2. **Remove all duplicate using directives** (lines 14, 16, 24, 26, 34-36, 40-43, 46, 48-50, 54-56)

3. **Alphabetically order all using directives** - System namespaces first, then third-party (FakeItEasy, NodaTime, Xunit), then project namespaces (Squidex.*)

    [Fact]
    public async Task Should_throw_when_appId_is_null()
    {
        // Arrange
        var mockRequestLogStore = A.Fake<IRequestLogStore>();
        var sut = new DefaultAppLogStore(mockRequestLogStore);
        var fromTime = Instant.FromUnixTimeSeconds(1000);
        var toTime = Instant.FromUnixTimeSeconds(2000);
        var stream = new MemoryStream();
    
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.ReadLogAsync(null!, fromTime, toTime, stream));
    }

*/

    [Fact]
    public async Task Should_return_null_for_unparseable_numeric_values()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = Instant.FromUnixTimeSeconds(1000);
        var toTime = Instant.FromUnixTimeSeconds(2000);
        var stream = new MemoryStream();
        
        var request = new Request
        {
            Key = appId.ToString(),
            Timestamp = Instant.FromUnixTimeSeconds(1500),
            Properties = new Dictionary<string, string>
            {
                ["RequestPath"] = "/api/test",
                ["Costs"] = "invalid_double",
                ["Bytes"] = "invalid_long",
                ["StatusCode"] = "not_a_number"
            }
        };
        
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, A<CancellationToken>._))
            .Returns(new[] { request }.ToAsyncEnumerable());
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream);
    
        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.Contains("/api/test", lines[1], StringComparison.Ordinal);
    }

/*
FAILED TEST: ## Analysis

The test compilation **failed** due to **1 critical error** and **multiple code style warnings**.

### Critical Error

**Line 85**: `AsyncEnumerable.Create<Request>(async (yield, ct) => ...)` - The delegate signature is incorrect. `AsyncEnumerable.Create` expects a `Func<CancellationToken, IAsyncEnumerator<Request>>` (1 argument), but the code provides a lambda with 2 parameters `(yield, ct)`.

### Code Style Warnings

1. **Multiple duplicate using directives** (lines 19, 21, 29-31, 35-37, 41, 43-45, 49-51)
2. **Using directives not alphabetically ordered** (lines 8, 11-17, 22-27, 33, 37, 39, 46-47, 50)

## Recommended Fixes

1. **Fix AsyncEnumerable.Create usage at line 85**:
   ```csharp
   .Returns(AsyncEnumerable.Create<Request>(ct =>
   {
       return new SingleItemAsyncEnumerator<Request>(request);
   }));
   ```
   Or use a simpler approach:
   ```csharp
   .Returns(new[] { request }.ToAsyncEnumerable());
   ```

2. **Remove all duplicate using directives** (lines 19, 21, 29-31, 35-37, 41, 43-45, 49-51)

3. **Alphabetically order all using directives** - System namespaces first, then third-party (FakeItEasy, NodaTime, Xunit), then project namespaces (Squidex.*)

    [Fact]
    public async Task Should_handle_requests_with_missing_properties()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = Instant.FromUnixTimeSeconds(1000);
        var toTime = Instant.FromUnixTimeSeconds(2000);
        var stream = new MemoryStream();
        
        var request = new Request
        {
            Key = appId.ToString(),
            Timestamp = Instant.FromUnixTimeSeconds(1500),
            Properties = new Dictionary<string, string>
            {
                ["RequestPath"] = "/api/test"
            }
        };
        
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, A<CancellationToken>._))
            .Returns(AsyncEnumerable.Create<Request>(async (yield, ct) =>
            {
                await yield.ReturnAsync(request);
            }));
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream);
    
        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.Contains("/api/test", lines[1], StringComparison.Ordinal);
    }

*/

    [Fact]
    public async Task Should_write_only_header_when_no_log_entries_exist()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = Instant.FromUnixTimeSeconds(1000);
        var toTime = Instant.FromUnixTimeSeconds(2000);
        var stream = new MemoryStream();
        
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, A<CancellationToken>._))
            .Returns(AsyncEnumerable.Empty<Request>());
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream);
    
        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines);
        Assert.Contains("Timestamp", lines[0], StringComparison.Ordinal);
        Assert.Contains("RequestPath", lines[0], StringComparison.Ordinal);
        Assert.Contains("StatusCode", lines[0], StringComparison.Ordinal);
    }


    [Fact]
    public async Task Should_not_log_whitespace_only_strings()
    {
        // Arrange
        var requestLogStore = A.Fake<IRequestLogStore>();
        A.CallTo(() => requestLogStore.IsEnabled).Returns(true);
        
        var sut = new DefaultAppLogStore(requestLogStore);
        
        var appId = DomainId.NewGuid();
        var timestamp = SystemClock.Instance.GetCurrentInstant();
        var requestLog = new RequestLog
        {
            UserClientId = "   ",
            UserId = "\t",
            CacheServer = "\n",
            CacheStatus = "  \t  ",
            RequestMethod = "GET",
            RequestPath = "/api/test",
            StatusCode = 200,
            Timestamp = timestamp
        };
    
        Request? capturedRequest = null;
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes((Request r, CancellationToken ct) => capturedRequest = r)
            .Returns(Task.CompletedTask);
    
        // Act
        await sut.LogAsync(appId, requestLog);
    
        // Assert
        Assert.NotNull(capturedRequest);
        Assert.False(capturedRequest.Properties.ContainsKey("AuthClientId"));
        Assert.False(capturedRequest.Properties.ContainsKey("AuthUserId"));
        Assert.False(capturedRequest.Properties.ContainsKey("CacheServer"));
        Assert.False(capturedRequest.Properties.ContainsKey("CacheStatus"));
        Assert.True(capturedRequest.Properties.ContainsKey("RequestMethod"));
        Assert.True(capturedRequest.Properties.ContainsKey("RequestPath"));
        Assert.Equal("GET", capturedRequest.Properties["RequestMethod"]);
        Assert.Equal("/api/test", capturedRequest.Properties["RequestPath"]);
    }

/*
FAILED TEST: ## Analysis

The test compilation **failed** due to **6 critical type conversion errors** where `null` is being assigned to non-nullable value types in the `RequestLog` object initialization.

### Critical Errors

Lines 56-57, 60-62, 65: Cannot convert `null` to non-nullable value types (`long`, `int`, `double`). The following properties don't accept `null`:
- `Bytes` (long)
- `CacheHits` (long) 
- `CacheTTL` (int)
- `Costs` (double)
- `ElapsedMs` (long)
- `StatusCode` (int)

### Code Style Issues
- Multiple duplicate using directives (lines 14, 16, 20, 22-24, 28-30)
- Using directives not alphabetically ordered
- Multiple blank lines (line 86)
- Trailing whitespace on multiple lines

## Recommended Fixes

1. **Remove null assignments** - Either:
   - Remove the properties with null values from the test object initialization
   - Use nullable types if the `RequestLog` class supports them (e.g., `Bytes = null` → remove the line entirely)
   - Assign default values instead of null (e.g., `Bytes = 0`, `Costs = 0.0`)

2. **Remove all duplicate using directives** (lines 14, 16, 20, 22-24, 28-30)

3. **Alphabetically order using directives** - System namespaces first, then third-party, then project namespaces

4. **Remove trailing whitespace** from all affected lines

5. **Remove multiple blank lines** (line 86)

    [Fact]
    public async Task Should_log_request_with_null_optional_fields()
    {
        // Arrange
        A.CallTo(() => requestLogStore.IsEnabled).Returns(true);
        
        var appId = DomainId.NewGuid();
        var timestamp = SystemClock.Instance.GetCurrentInstant();
        var requestLog = new RequestLog
        {
            UserClientId = null,
            UserId = null,
            Bytes = null,
            CacheHits = null,
            CacheServer = null,
            CacheStatus = null,
            CacheTTL = null,
            Costs = null,
            ElapsedMs = null,
            RequestMethod = null,
            RequestPath = null,
            StatusCode = null,
            Timestamp = timestamp
        };
    
        Request? capturedRequest = null;
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes((Request r, CancellationToken ct) => capturedRequest = r)
            .Returns(Task.CompletedTask);
    
        // Act
        await sut.LogAsync(appId, requestLog);
    
        // Assert
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        
        Assert.NotNull(capturedRequest);
        Assert.Equal(appId.ToString(), capturedRequest.Key);
        Assert.Equal(timestamp, capturedRequest.Timestamp);
        Assert.Empty(capturedRequest.Properties);
    }

*/

    [Fact]
    public async Task Should_not_log_when_store_is_disabled()
    {
        // Arrange
        var requestLogStore = A.Fake<IRequestLogStore>();
        A.CallTo(() => requestLogStore.IsEnabled).Returns(false);
        
        var sut = new DefaultAppLogStore(requestLogStore);
        
        var appId = DomainId.NewGuid();
        var requestLog = new RequestLog
        {
            UserClientId = "client123",
            RequestMethod = "GET",
            RequestPath = "/api/test",
            StatusCode = 200,
            Timestamp = SystemClock.Instance.GetCurrentInstant()
        };
    
        // Act
        await sut.LogAsync(appId, requestLog);
    
        // Assert
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

/*
FAILED TEST: ## Analysis

The test compilation **failed** with one critical error and multiple code style warnings.

### Critical Error
- **Line 39**: `Mocks.App(...)` - The `Mocks` class does not contain a definition for `App`. This method doesn't exist in the test helper class.

### Code Style Warnings
1. **Duplicate using directives** (lines 19-21): `Squidex.Infrastructure`, `Squidex.Infrastructure.Log`, and `NodaTime` appear twice
2. **Using directives not alphabetically ordered** (multiple lines)
3. **Multiple blank lines** (line 52)
4. **Trailing whitespace** (lines 41, 44, 47, 63, 96, 99, 101, 104, 109, 155, 174, 179, 182, 186)
5. **Missing trailing commas** in dictionaries and collections
6. **Blank line before closing brace** (line 206)
7. **String comparison without StringComparison parameter** (lines 111-116)

## Recommended Fixes

1. **Replace `Mocks.App(...)` with correct mock creation method** - Check the `Mocks` class or `GivenContext` for the proper way to create an `App` instance (likely `Mocks.Apps(...)` or similar)
2. **Remove duplicate using statements** (lines 19-21)
3. **Alphabetically order all using directives**
4. **Remove all trailing whitespace**
5. **Remove multiple consecutive blank lines**
6. **Add trailing commas to dictionary/collection initializers**
7. **Remove blank line before closing brace** (line 206)
8. **Add `StringComparison` parameter to `Contains()` calls**

    [Fact]
    public async Task Should_delete_app_logs_via_deleter_interface()
    {
        // Arrange
        var app = Mocks.App(NamedId.Of(DomainId.NewGuid(), "test-app"));
        var deleter = (IDeleter)sut;
    
        A.CallTo(() => requestLogStore.DeleteAsync(A<string>._, A<CancellationToken>._))
            .Returns(Task.CompletedTask);
    
        // Act
        await deleter.DeleteAppAsync(app, CancellationToken.None);
    
        // Assert
        A.CallTo(() => requestLogStore.DeleteAsync(app.Id.ToString(), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

*/

    [Fact]
    public async Task Should_read_log_and_write_to_csv_stream()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = Instant.FromUnixTimeSeconds(1000);
        var toTime = Instant.FromUnixTimeSeconds(2000);
        var timestamp1 = Instant.FromUnixTimeSeconds(1500);
        var timestamp2 = Instant.FromUnixTimeSeconds(1600);
    
        var requests = new[]
        {
            new Request
            {
                Timestamp = timestamp1,
                Properties = new Dictionary<string, string>
                {
                    ["RequestPath"] = "/api/test",
                    ["RequestMethod"] = "POST",
                    ["RequestElapsedMs"] = "150.5",
                    ["Costs"] = "0.25",
                    ["AuthClientId"] = "client1",
                    ["AuthUserId"] = "user1",
                    ["Bytes"] = "2048",
                    ["CacheHits"] = "3",
                    ["CacheServer"] = "server1",
                    ["CacheStatus"] = "MISS",
                    ["CacheTTL"] = "1800",
                    ["StatusCode"] = "201"
                }
            },
            new Request
            {
                Timestamp = timestamp2,
                Properties = new Dictionary<string, string>
                {
                    ["RequestPath"] = "/api/data",
                    ["RequestMethod"] = "GET",
                    ["StatusCode"] = "200"
                }
            }
        };
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, A<CancellationToken>._))
            .Returns(requests.ToAsyncEnumerable());
    
        var stream = new MemoryStream();
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream);
    
        // Assert
        stream.Position = 0;
        var csvContent = Encoding.UTF8.GetString(stream.ToArray());
        var lines = csvContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        Assert.Equal(3, lines.Length); // Header + 2 data rows
        Assert.Contains("Timestamp|RequestPath|RequestMethod", lines[0]);
        Assert.Contains(timestamp1.ToString(), lines[1]);
        Assert.Contains("/api/test", lines[1]);
        Assert.Contains("POST", lines[1]);
        Assert.Contains(timestamp2.ToString(), lines[2]);
        Assert.Contains("/api/data", lines[2]);
    }

/*
FAILED TEST: ## Analysis

The test compilation **failed** due to a type conversion error and several code style warnings.

### Critical Error
- **Line 48**: `ElapsedMs = 123.45` - Cannot implicitly convert `double` to `long`. The `ElapsedMs` property expects a `long` type, but a `double` value (123.45) was provided.

### Code Style Warnings
1. **Duplicate using directives** (lines 15-16): `Squidex.Infrastructure.Log` and `NodaTime` are imported twice
2. **Using directives not alphabetically ordered** (lines 12, 15)
3. **Multiple blank lines** (line 83)
4. **Trailing whitespace** (lines 35, 54, 59, 62, 66)
5. **Blank line before closing brace** (line 85)
6. **Missing trailing comma** (line 52)

## Recommended Fixes

1. **Fix the type error** on line 48:
   ```csharp
   ElapsedMs = 123  // Change to long, or cast: (long)123.45
   ```

2. **Remove duplicate using statements** (lines 15-16)

3. **Clean up formatting**:
   - Remove trailing whitespace
   - Remove extra blank lines
   - Add trailing comma after `Timestamp = timestamp`
   - Ensure using statements are alphabetically ordered

    [Fact]
    public async Task Should_log_complete_request_with_all_fields()
    {
        // Arrange
        A.CallTo(() => requestLogStore.IsEnabled).Returns(true);
        
        var appId = DomainId.NewGuid();
        var timestamp = SystemClock.Instance.GetCurrentInstant();
        var requestLog = new RequestLog
        {
            UserClientId = "client123",
            UserId = "user456",
            Bytes = 1024,
            CacheHits = 5,
            CacheServer = "cache-server-1",
            CacheStatus = "HIT",
            CacheTTL = 3600,
            Costs = 0.5,
            ElapsedMs = 123.45,
            RequestMethod = "GET",
            RequestPath = "/api/content",
            StatusCode = 200,
            Timestamp = timestamp
        };
    
        Request? capturedRequest = null;
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes((Request r, CancellationToken ct) => capturedRequest = r)
            .Returns(Task.CompletedTask);
    
        // Act
        await sut.LogAsync(appId, requestLog);
    
        // Assert
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        
        Assert.NotNull(capturedRequest);
        Assert.Equal(appId.ToString(), capturedRequest.Key);
        Assert.Equal(timestamp, capturedRequest.Timestamp);
        Assert.Equal("client123", capturedRequest.Properties["AuthClientId"]);
        Assert.Equal("user456", capturedRequest.Properties["AuthUserId"]);
        Assert.Equal("1024", capturedRequest.Properties["Bytes"]);
        Assert.Equal("5", capturedRequest.Properties["CacheHits"]);
        Assert.Equal("cache-server-1", capturedRequest.Properties["CacheServer"]);
        Assert.Equal("HIT", capturedRequest.Properties["CacheStatus"]);
        Assert.Equal("3600", capturedRequest.Properties["CacheTTL"]);
        Assert.Equal("0.5", capturedRequest.Properties["Costs"]);
        Assert.Equal("123.45", capturedRequest.Properties["RequestElapsedMs"]);
        Assert.Equal("GET", capturedRequest.Properties["RequestMethod"]);
        Assert.Equal("/api/content", capturedRequest.Properties["RequestPath"]);
        Assert.Equal("200", capturedRequest.Properties["StatusCode"]);
    }

*/

}
