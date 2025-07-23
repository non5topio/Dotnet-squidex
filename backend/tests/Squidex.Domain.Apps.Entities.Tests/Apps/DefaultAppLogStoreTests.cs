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

    [Fact]
    public void Should_run_deletion_in_default_order()
    {
        var order = ((IDeleter)sut).Order;

        Assert.Equal(0, order);
/*
FAILED TEST: The test run failed due to a **C# syntax error** in the file `DefaultAppLogStoreTests.cs`:

- The test method `Should_run_deletion_in_default_order` is **missing a closing brace (`}`)**.
- This causes the next method `Append_ShouldNotAddNullOrEmptyValues` to be interpreted as part of the same method body.
- The compiler throws the error:
  ```
  error CS0106: The modifier 'public' is not valid for this item
  ```

### **Recommended Fix:**
Add the missing closing brace (`}`) at the end of the `Should_run_deletion_in_default_order` method to properly close it before the next method declaration.

    [Fact]
    public async Task ReadLogAsync_ShouldHandleStringTimestamp()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Instance.GetCurrentInstant();
        var toTime = fromTime.Plus(Duration.FromDays(1));
        var stream = new MemoryStream();
        var request = new Request
        {
            Properties = new Dictionary<string, string>
            {
                { DefaultAppLogStore.FieldAuthClientId, "ClientId" },
                { DefaultAppLogStore.FieldAuthUserId, "UserId" },
                { DefaultAppLogStore.FieldBytes, "1024" },
                { DefaultAppLogStore.FieldCosts, "0.5" },
                { DefaultAppLogStore.FieldCacheStatus, "Hit" },
                { DefaultAppLogStore.FieldCacheServer, "Server1" },
                { DefaultAppLogStore.FieldCacheTTL, "3600" },
                { DefaultAppLogStore.FieldCacheHits, "5" },
                { DefaultAppLogStore.FieldRequestElapsedMs, "150" },
                { DefaultAppLogStore.FieldRequestMethod, "GET" },
                { DefaultAppLogStore.FieldRequestPath, "/api/test" },
                { DefaultAppLogStore.FieldStatusCode, "200" }
            },
            Timestamp = "2023-01-01T00:00:00Z"
        };
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, default))
            .Returns(new[] { request }.ToAsyncEnumerable());
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, default);
    
        // Assert
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
    
        Assert.Contains("2023-01-01T00:00:00Z", content);
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **C# syntax error** in the file `DefaultAppLogStoreTests.cs`. The method `Should_run_deletion_in_default_order` is **missing a closing brace (`}`)**, causing the next method to be interpreted as part of the same method body. This leads to the compiler error:
```
error CS0106: The modifier 'public' is not valid for this item
```

**Recommended Fix:**  
Add the missing closing brace (`}`) at the end of the `Should_run_deletion_in_default_order` method to properly close it before the next method declaration.

    [Fact]
    public async Task ReadLogAsync_ShouldWriteHeader_WhenNoEntriesFound()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Instance.GetCurrentInstant();
        var toTime = fromTime.Plus(Duration.FromDays(1));
        var stream = new MemoryStream();
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, default))
            .Returns(Enumerable.Empty<Request>().ToAsyncEnumerable());
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, default);
    
        // Assert
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
    
        Assert.Contains(DefaultAppLogStore.FieldTimestamp, content);
        Assert.Contains(DefaultAppLogStore.FieldRequestPath, content);
        Assert.Contains(DefaultAppLogStore.FieldRequestMethod, content);
        Assert.Contains(DefaultAppLogStore.FieldRequestElapsedMs, content);
        Assert.Contains(DefaultAppLogStore.FieldCosts, content);
        Assert.Contains(DefaultAppLogStore.FieldAuthClientId, content);
        Assert.Contains(DefaultAppLogStore.FieldAuthUserId, content);
        Assert.Contains(DefaultAppLogStore.FieldBytes, content);
        Assert.Contains(DefaultAppLogStore.FieldCacheHits, content);
        Assert.Contains(DefaultAppLogStore.FieldCacheServer, content);
        Assert.Contains(DefaultAppLogStore.FieldCacheStatus, content);
        Assert.Contains(DefaultAppLogStore.FieldCacheTTL, content);
        Assert.Contains(DefaultAppLogStore.FieldStatusCode, content);
    }

*/
/*
FAILED TEST: The test run failed due to a **C# syntax error** in the file `backend/tests/Squidex.Domain.Apps.Entities.Tests/Apps/DefaultAppLogStoreTests.cs`. Specifically:

- The test method `Should_run_deletion_in_default_order` is **missing a closing brace (`}`)**.
- This causes the following method `Append_ShouldNotAddNullOrEmptyValues` to be incorrectly interpreted as part of the previous method body.
- The compiler then throws the error:
  ```
  error CS0106: The modifier 'public' is not valid for this item
  ```

### **Recommended Fix:**
Add the missing closing brace (`}`) at the end of the `Should_run_deletion_in_default_order` method to properly close it before the next method declaration.

    [Fact]
    public async Task ReadLogAsync_ShouldWriteAllFields_WhenAllArePopulated()
    {
        // Arrange
        var appId = DomainId.NewGuid();
        var fromTime = SystemClock.Instance.GetCurrentInstant();
        var toTime = fromTime.Plus(Duration.FromDays(1));
        var stream = new MemoryStream();
        var request = new Request
        {
            Properties = new Dictionary<string, string>
            {
                { DefaultAppLogStore.FieldAuthClientId, "ClientId" },
                { DefaultAppLogStore.FieldAuthUserId, "UserId" },
                { DefaultAppLogStore.FieldBytes, "1024" },
                { DefaultAppLogStore.FieldCosts, "0.5" },
                { DefaultAppLogStore.FieldCacheStatus, "Hit" },
                { DefaultAppLogStore.FieldCacheServer, "Server1" },
                { DefaultAppLogStore.FieldCacheTTL, "3600" },
                { DefaultAppLogStore.FieldCacheHits, "5" },
                { DefaultAppLogStore.FieldRequestElapsedMs, "150" },
                { DefaultAppLogStore.FieldRequestMethod, "GET" },
                { DefaultAppLogStore.FieldRequestPath, "/api/test" },
                { DefaultAppLogStore.FieldStatusCode, "200" }
            },
            Timestamp = fromTime
        };
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), fromTime, toTime, default))
            .Returns(new[] { request }.ToAsyncEnumerable());
    
        // Act
        await sut.ReadLogAsync(appId, fromTime, toTime, stream, default);
    
        // Assert
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
    
        Assert.Contains(DefaultAppLogStore.FieldTimestamp, content);
        Assert.Contains(DefaultAppLogStore.FieldRequestPath, content);
        Assert.Contains(DefaultAppLogStore.FieldRequestMethod, content);
        Assert.Contains(DefaultAppLogStore.FieldRequestElapsedMs, content);
        Assert.Contains(DefaultAppLogStore.FieldCosts, content);
        Assert.Contains(DefaultAppLogStore.FieldAuthClientId, content);
        Assert.Contains(DefaultAppLogStore.FieldAuthUserId, content);
        Assert.Contains(DefaultAppLogStore.FieldBytes, content);
        Assert.Contains(DefaultAppLogStore.FieldCacheHits, content);
        Assert.Contains(DefaultAppLogStore.FieldCacheServer, content);
        Assert.Contains(DefaultAppLogStore.FieldCacheStatus, content);
        Assert.Contains(DefaultAppLogStore.FieldCacheTTL, content);
        Assert.Contains(DefaultAppLogStore.FieldStatusCode, content);
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **C# syntax error** in the file `DefaultAppLogStoreTests.cs`. Specifically, the test method `Should_run_deletion_in_default_order` is missing a closing brace (`}`), which causes the subsequent method to be incorrectly interpreted as part of the same method body. This results in the compiler error:

```
error CS0106: The modifier 'public' is not valid for this item
```

**Recommended Fix:**  
Add the missing closing brace (`}`) to the end of the `Should_run_deletion_in_default_order` method to properly close it before the next method declaration.

    [Fact]
    public void GetDoubleAndGetLong_ShouldReturnNullForNonNumericValues()
    {
        // Arrange
        var request = new Request
        {
            Properties = new Dictionary<string, string>
            {
                { "DoubleField", "NotANumber" },
                { "LongField", "InvalidLong" }
            }
        };
    
        // Act & Assert
        Assert.Null(sut.GetDouble(request, "DoubleField"));
        Assert.Null(sut.GetLong(request, "LongField"));
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **C# syntax error** in the test file `DefaultAppLogStoreTests.cs`. The method `Should_run_deletion_in_default_order` is missing a closing brace (`}`), causing the next method to be incorrectly interpreted as part of the previous one. This leads to an invalid use of the `public` modifier, resulting in the error:

```
error CS0106: The modifier 'public' is not valid for this item
```

**Recommended Fix:**  
Add the missing closing brace (`}`) to properly close the `Should_run_deletion_in_default_order` method before the next test method.

    [Fact]
    public void Append_ShouldNotAddNullOrEmptyValues()
    {
        // Arrange
        var request = new Request();
    
        // Act
        sut.Append(request, "TestKey1", (string)null);
        sut.Append(request, "TestKey2", string.Empty);
        sut.Append(request, "TestKey3", (object)null);
        sut.Append(request, "TestKey4", "");
    
        // Assert
        Assert.False(request.Properties.ContainsKey("TestKey1"));
        Assert.False(request.Properties.ContainsKey("TestKey2"));
        Assert.False(request.Properties.ContainsKey("TestKey3"));
        Assert.False(request.Properties.ContainsKey("TestKey4"));
    }

*/
/*
FAILED TEST: **Analysis:**  
The test run failed due to a **C# syntax error** in the test file `DefaultAppLogStoreTests.cs`. The method `Should_run_deletion_in_default_order` is missing a closing brace (`}`), causing the next method to be incorrectly interpreted as part of the previous one. This leads to an invalid use of the `public` modifier, resulting in the error:

```
error CS0106: The modifier 'public' is not valid for this item
```

**Recommended Fix:**  
Add the missing closing brace (`}`) to properly close the `Should_run_deletion_in_default_order` method before the next test method.

    [Fact]
    public async Task ReadLogAsync_ShouldThrowException_WhenAppIdIsNull()
    {
        // Arrange
        DomainId appId = null!;
        var fromTime = SystemClock.Instance.GetCurrentInstant();
        var toTime = fromTime.Plus(Duration.FromDays(1));
        var stream = new MemoryStream();
    
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ReadLogAsync(appId, fromTime, toTime, default));
    }

*/
/*
FAILED TEST: The test run failed due to a **C# syntax error** in the test file `DefaultAppLogStoreTests.cs`:

- **Error**: `error CS0106: The modifier 'public' is not valid for this item`
- **Location**: Line 34, column 5 of the file.

### Root Cause:
The `[Fact]` test method `Should_run_deletion_in_default_order` is missing a closing brace (`}`) for the method body. As a result, the next test method `LogAsync_ShouldReturnCompletedTask_WhenLoggingIsDisabled` is incorrectly interpreted as being part of the previous method, leading to a syntax error when the `public` modifier is applied to it.

### Recommended Fix:
Add the missing closing brace (`}`) to properly close the `Should_run_deletion_in_default_order` method before the next test method.

**Fix:**
```csharp
    [Fact]
    public void Should_run_deletion_in_default_order()
    {
        var order = ((IDeleter)sut).Order;

        Assert.Equal(0, order);
    } // <-- Add this closing brace

    [Fact]
    public async Task LogAsync_ShouldReturnCompletedTask_WhenLoggingIsDisabled()
    {
        // Arrange
        A.CallTo(() => requestLogStore.IsEnabled).Returns(false);
        var appId = DomainId.NewGuid();
        var request = new RequestLog();
    
        // Act
        await sut.LogAsync(appId, request, default);
    
        // Assert
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, default)).MustNotHaveHappened();
    }
```

    [Fact]
    public async Task LogAsync_ShouldReturnCompletedTask_WhenLoggingIsDisabled()
    {
        // Arrange
        A.CallTo(() => requestLogStore.IsEnabled).Returns(false);
        var appId = DomainId.NewGuid();
        var request = new RequestLog();
    
        // Act
        await sut.LogAsync(appId, request, default);
    
        // Assert
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, default)).MustNotHaveHappened();
    }

*/
    }
}
