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
    }

    [Fact]
    public async Task Should_remove_events_from_streams()
    {
        await ((IDeleter)sut).DeleteAppAsync(App, CancellationToken);

        A.CallTo(() => requestLogStore.DeleteAsync($"^[a-z]-{AppId.Id}", A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_not_forward_request_if_disabled()
    {
        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(false);

        await sut.LogAsync(AppId.Id, default, CancellationToken);

        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_forward_request_log_to_store()
    {
        Request? recordedRequest = null;

        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(true);

        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, CancellationToken))
            .Invokes(x => recordedRequest = x.GetArgument<Request>(0)!);

        var request = default(RequestLog);
        request.Bytes = 1024;
        request.CacheHits = 10;
        request.CacheServer = "server-fra";
        request.CacheStatus = "MISS";
        request.CacheTTL = 3600;
        request.Costs = 1.5;
        request.ElapsedMs = 120;
        request.RequestMethod = "GET";
        request.RequestPath = "/my-path";
        request.StatusCode = 200;
        request.Timestamp = default;
        request.UserClientId = "frontend";
        request.UserId = "user1";

        await sut.LogAsync(AppId.Id, request, CancellationToken);

        Assert.NotNull(recordedRequest);

        Contains(request.Bytes, recordedRequest);
        Contains(request.CacheHits, recordedRequest);
        Contains(request.CacheServer, recordedRequest);
        Contains(request.CacheStatus, recordedRequest);
        Contains(request.CacheTTL, recordedRequest);
        Contains(request.ElapsedMs.ToString(CultureInfo.InvariantCulture), recordedRequest);
        Contains(request.RequestMethod, recordedRequest);
        Contains(request.RequestPath, recordedRequest);
        Contains(request.StatusCode, recordedRequest);
        Contains(request.UserClientId, recordedRequest);
        Contains(request.UserId, recordedRequest);

        Assert.Equal(AppId.Id.ToString(), recordedRequest?.Key);
    }

    [Fact]
    public async Task Should_write_to_stream()
    {
        var timeFrom = SystemClock.Instance.GetCurrentInstant();
        var timeTo = timeFrom.Plus(Duration.FromDays(4));

        A.CallTo(() => requestLogStore.QueryAllAsync(AppId.Id.ToString(), timeFrom, timeTo, CancellationToken))
            .Returns(new[]
            {
                CreateRecord(),
                CreateRecord(),
                CreateRecord(),
                CreateRecord(),
            }.ToAsyncEnumerable());

        var stream = new MemoryStream();

        await sut.ReadLogAsync(AppId.Id, timeFrom, timeTo, stream, CancellationToken);
        stream.Position = 0;

        var lines = 0;
        using (var reader = new StreamReader(stream))
        {
            while (await reader.ReadLineAsync(default) != null)
            {
                lines++;
            }
        }

        Assert.Equal(5, lines);
    }

    private static void Contains(string value, Request? request)
    {
        Assert.Contains(value, request!.Properties.Values);
    }

    private static void Contains(object value, Request? request)
    {
        Assert.Contains(Convert.ToString(value, CultureInfo.InvariantCulture), request!.Properties.Values);
    }

/*
FAILED TEST: **Analysis of Test Failure:**

The test run failed due to **C# compilation errors** in the test file `DefaultAppLogStoreTests.cs`, specifically:

1. **Missing field references**:
   - Fields like `FieldTimestamp`, `FieldRequestPath`, `FieldRequestMethod`, etc., are referenced in the test but are defined as `private const` in the implementation class `DefaultAppLogStore`.
   - This results in compiler errors like `CS0117`: `'DefaultAppLogStore' does not contain a definition for 'FieldX'`.

2. **Syntax and formatting issues**:
   - Missing semicolons (`;`) at the end of statements.
   - Invalid token `{` and incorrect code block placements, indicating malformed methods or class members.

3. **Duplicate test method**:
   - A duplicate test method `Should_not_forward_request_if_disabled` exists in the test class, causing a `CS0111` error.

---

**Recommended Fixes:**

1. **Access constants via the class name**:
   - Replace all references like `FieldTimestamp` with `DefaultAppLogStore.FieldTimestamp`, and similarly for other fields.

2. **Correct syntax errors**:
   - Add missing semicolons at the end of statements.
   - Fix incorrect code block placements and malformed methods.

3. **Resolve duplicate method**:
   - Rename or remove the duplicate test method `Should_not_forward_request_if_disabled`.

These fixes will resolve the compilation errors and allow the test to run successfully.

    [Fact]
    public async Task Should_write_max_records_to_stream()
    {
        var timeFrom = SystemClock.Instance.GetCurrentInstant();
        var timeTo = timeFrom.Plus(Duration.FromDays(4));
    
        var requests = new List<Request>();
        for (int i = 0; i < 1000; i++)
        {
            requests.Add(new Request
            {
                Properties = new Dictionary<string, string>
                {
                    { DefaultAppLogStore.FieldTimestamp, "2023-01-01T00:00:00Z" },
                    { DefaultAppLogStore.FieldRequestPath, "/api/test" },
                    { DefaultAppLogStore.FieldRequestMethod, "GET" },
                    { DefaultAppLogStore.FieldRequestElapsedMs, "100" },
                    { DefaultAppLogStore.FieldCosts, "0.5" },
                    { DefaultAppLogStore.FieldAuthClientId, "client1" },
                    { DefaultAppLogStore.FieldAuthUserId, "user1" },
                    { DefaultAppLogStore.FieldBytes, "1024" },
                    { DefaultAppLogStore.FieldCacheHits, "1" },
                    { DefaultAppLogStore.FieldCacheServer, "server1" },
                    { DefaultAppLogStore.FieldCacheStatus, "HIT" },
                    { DefaultAppLogStore.FieldCacheTTL, "3600" },
                    { DefaultAppLogStore.FieldStatusCode, "200" }
                },
                Timestamp = SystemClock.Instance.GetCurrentInstant()
            });
        }
    
        A.CallTo(() => requestLogStore.QueryAllAsync(AppId.Id.ToString(), timeFrom, timeTo, CancellationToken))
            .Returns(requests.ToAsyncEnumerable());
    
        var stream = new MemoryStream();
    
        await sut.ReadLogAsync(AppId.Id, timeFrom, timeTo, stream, CancellationToken);
        stream.Position = 0;
    
        var lines = 0;
        using (var reader = new StreamReader(stream))
        {
            while (await reader.ReadLineAsync() != null)
            {
                lines++;
            }
        }
    
        Assert.Equal(1001, lines); // 1 header line + 1000 data lines
    }

*/
/*
FAILED TEST: The test run failed due to a **C# compilation error** in the file `DefaultAppLogStoreTests.cs`:

### **Reason for Failure:**
- **Error**: `CS0111: Type 'DefaultAppLogStoreTests' already defines a member called 'Should_not_forward_request_if_disabled' with the same parameter types`
- This indicates a **duplicate test method** with the same name and signature already exists in the test class.

### **Recommended Fix:**
- Rename or remove the duplicate test method `Should_not_forward_request_if_disabled` to resolve the naming conflict and allow successful compilation.

    [Fact]
    public async Task Should_not_forward_request_if_disabled()
    {
        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(false);
    
        await sut.LogAsync(AppId.Id, default, CancellationToken);
    
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

*/
/*
FAILED TEST: The test run failed due to **C# compilation errors** in the test file `DefaultAppLogStoreTests.cs`, specifically:

### **Reasons for Failure:**
1. **Missing field references**:
   - Fields like `FieldRequestElapsedMs`, `FieldCosts`, `FieldBytes`, etc., are referenced in the test but are defined as `private const` in the implementation class `DefaultAppLogStore`.
   - This results in compiler errors like `CS0117`: `'DefaultAppLogStore' does not contain a definition for 'FieldX'`.

### **Recommended Fixes:**
1. **Access constants via the class name**:
   - Use `DefaultAppLogStore.FieldRequestElapsedMs`, `DefaultAppLogStore.FieldCosts`, etc., to reference the constants properly.
2. **Fix syntax and formatting issues**:
   - Add missing semicolons at the end of statements.
   - Correct malformed code blocks and ensure valid method/block structure.

    [Fact]
    public async Task Should_return_null_for_invalid_data_in_GetDouble_and_GetLong()
    {
        var timeFrom = SystemClock.Instance.GetCurrentInstant();
        var timeTo = timeFrom.Plus(Duration.FromDays(4));
    
        var request = new Request
        {
            Properties = new Dictionary<string, string>
            {
                { DefaultAppLogStore.FieldRequestElapsedMs, "not a number" },
                { DefaultAppLogStore.FieldCosts, "invalid" },
                { DefaultAppLogStore.FieldBytes, "abc" },
                { DefaultAppLogStore.FieldCacheHits, "xyz" },
                { DefaultAppLogStore.FieldCacheTTL, "123.45" },
                { DefaultAppLogStore.FieldStatusCode, "not a long" }
            }
        };
    
        A.CallTo(() => requestLogStore.QueryAllAsync(AppId.Id.ToString(), timeFrom, timeTo, CancellationToken.None))
            .Returns(new[] { request }.ToAsyncEnumerable());
    
        var stream = new MemoryStream();
    
        await sut.ReadLogAsync(AppId.Id, timeFrom, timeTo, stream, CancellationToken.None);
        stream.Position = 0;
    
        using (var reader = new StreamReader(stream))
        {
            var header = await reader.ReadLineAsync();
            var dataLine = await reader.ReadLineAsync();
    
            Assert.NotNull(header);
            Assert.NotNull(dataLine);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **C# compilation errors** in the test file `DefaultAppLogStoreTests.cs`, specifically:

### **Reasons for Failure:**
1. **Missing field references**:
   - Fields like `FieldBytes`, `FieldCacheHits`, etc., are referenced in the test but are defined as `private const` in the implementation class `DefaultAppLogStore`. These should be accessed using the class name (e.g., `DefaultAppLogStore.FieldBytes`).
2. **Syntax and formatting issues**:
   - Missing semicolons (`;`) at the end of statements.
   - Invalid token `{` and incorrect code block placements, indicating malformed methods or class members.

### **Recommended Fixes:**
1. **Access constants via the class name**:
   - Replace direct field references with `DefaultAppLogStore.FieldBytes`, `DefaultAppLogStore.FieldCacheHits`, etc.
2. **Correct syntax errors**:
   - Add missing semicolons at the end of statements.
   - Fix incorrect code block placements and ensure valid method/block structure.
3. **Ensure proper method declarations**:
   - Ensure all methods have correct syntax and valid braces `{}` placement.

    [Fact]
    public async Task Should_write_correct_number_of_records_to_stream()
    {
        var timeFrom = SystemClock.Instance.GetCurrentInstant();
        var timeTo = timeFrom.Plus(Duration.FromDays(4));
    
        A.CallTo(() => requestLogStore.QueryAllAsync(AppId.Id.ToString(), timeFrom, timeTo, CancellationToken))
            .Returns(Enumerable.Empty<Request>().ToAsyncEnumerable());
    
        var stream = new MemoryStream();
    
        await sut.ReadLogAsync(AppId.Id, timeFrom, timeTo, stream, CancellationToken);
        stream.Position = 0;
    
        var lines = 0;
        using (var reader = new StreamReader(stream))
        {
            while (await reader.ReadLineAsync() != null)
            {
                lines++;
            }
        }
    
        Assert.Equal(1, lines); // Only the header line
    
        A.CallTo(() => requestLogStore.QueryAllAsync(AppId.Id.ToString(), timeFrom, timeTo, CancellationToken))
            .Returns(new[] { CreateRecord() }.ToAsyncEnumerable());
    
        stream = new MemoryStream();
    
        await sut.ReadLogAsync(AppId.Id, timeFrom, timeTo, stream, CancellationToken);
        stream.Position = 0;
    
        lines = 0;
        using (var reader = new StreamReader(stream))
        {
            while (await reader.ReadLineAsync() != null)
            {
                lines++;
            }
        }
    
        Assert.Equal(2, lines); // Header + one data line
    }

*/
/*
FAILED TEST: The test run failed due to **C# compilation errors** in the test file `DefaultAppLogStoreTests.cs`, specifically:

### **Reasons for Failure:**
1. **Missing field references**:
   - Fields like `FieldAuthClientId`, `FieldAuthUserId`, etc., are referenced in the test but are not accessible because they are defined as `private const` in the implementation class `DefaultAppLogStore`.
2. **Syntax and formatting issues**:
   - Missing semicolons (`;`) at the end of statements.
   - Invalid token `{` and incorrect code block placements, indicating malformed methods or class members.

### **Recommended Fixes:**
1. **Access constants via the class name**:
   - Use `DefaultAppLogStore.FieldAuthClientId`, `DefaultAppLogStore.FieldAuthUserId`, etc., to reference the constants.
2. **Fix syntax errors**:
   - Add missing semicolons at the end of statements.
   - Correct malformed code blocks by ensuring proper placement of braces `{}` and valid method declarations.
3. **Remove trailing whitespace and fix code formatting**:
   - Address the `SA1028` warnings by removing trailing whitespace from lines.

    [Fact]
    public async Task Should_not_add_null_or_empty_string_fields_to_request()
    {
        Request? recordedRequest = null;
    
        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(true);
    
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes(x => recordedRequest = x.GetArgument<Request>(0)!);
    
        var request = new RequestLog
        {
            UserClientId = null,
            UserId = string.Empty,
            RequestPath = null,
            RequestMethod = null,
            CacheServer = null,
            CacheStatus = string.Empty,
            Timestamp = default
        };
    
        await sut.LogAsync(AppId.Id, request, CancellationToken.None);
    
        Assert.NotNull(recordedRequest);
    
        Assert.DoesNotContain(DefaultAppLogStore.FieldAuthClientId, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldAuthUserId, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldRequestPath, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldRequestMethod, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldCacheServer, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldCacheStatus, recordedRequest.Properties);
    }

*/
/*
FAILED TEST: The test run failed due to **C# compilation errors** in the test file `DefaultAppLogStoreTests.cs`, specifically:

### **Reasons for Failure:**
1. **Type conversion errors**:
   - `CS0029`: Cannot implicitly convert type `string` to `long`, `int`, or `double` — explicit casting is required.
2. **Missing field references**:
   - `CS0117`: Fields like `FieldBytes`, `FieldCacheHits`, etc., are referenced but not defined in the test class — these constants exist in the implementation class but are not accessible in the test.
3. **Syntax and formatting issues**:
   - Missing semicolons and incorrect code block placements prevent successful compilation.

### **Recommended Fixes:**
1. **Fix type conversion errors**:
   - Explicitly cast string values to the required numeric types (e.g., `long.Parse`, `int.Parse`, `double.Parse`).
2. **Reference constants from the implementation class**:
   - Use `DefaultAppLogStore.FieldBytes`, `DefaultAppLogStore.FieldCacheHits`, etc., to access the constants.
3. **Correct syntax errors**:
   - Add missing semicolons and ensure correct method/block structure in the test file.

These fixes will allow the test file to compile and execute successfully.

    [Fact]
    public async Task Should_not_add_non_numeric_values_to_request()
    {
        Request? recordedRequest = null;
    
        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(true);
    
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes(x => recordedRequest = x.GetArgument<Request>(0)!);
    
        var request = new RequestLog
        {
            Bytes = "not a number",
            CacheHits = "invalid",
            CacheTTL = "abc",
            Costs = "xyz",
            ElapsedMs = "120.5",
            StatusCode = "not a long",
            Timestamp = default
        };
    
        await sut.LogAsync(AppId.Id, request, CancellationToken.None);
    
        Assert.NotNull(recordedRequest);
    
        Assert.DoesNotContain(DefaultAppLogStore.FieldBytes, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldCacheHits, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldCacheTTL, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldCosts, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldRequestElapsedMs, recordedRequest.Properties);
        Assert.DoesNotContain(DefaultAppLogStore.FieldStatusCode, recordedRequest.Properties);
    }

*/
/*
FAILED TEST: The test run failed due to **C# compilation errors** in the test file `DefaultAppLogStoreTests.cs`, specifically:

### **Reasons for Failure:**
1. **Type conversion errors**:
   - `CS0266`: Cannot implicitly convert type `long` to `int` or `double` to `long` — explicit casting is required.
2. **Missing field references**:
   - `CS0117`: Fields like `FieldBytes`, `FieldCacheHits`, etc., are referenced but not defined in the test class — these constants exist in the implementation class but are not accessible in the test.
3. **Syntax and formatting issues**:
   - Missing semicolons and incorrect code block placements prevent successful compilation.

### **Recommended Fixes:**
1. **Fix type mismatches**:
   - Add explicit casts (e.g., `(int)longValue`, `(long)doubleValue`) where necessary.
2. **Access constants properly**:
   - Reference the constants from the `DefaultAppLogStore` class using the class name (e.g., `DefaultAppLogStore.FieldBytes`).
3. **Correct syntax errors**:
   - Add missing semicolons and ensure correct method/block structure in the test file.

These fixes will allow the test file to compile and execute successfully.

    [Fact]
    public async Task Should_add_max_values_to_request()
    {
        Request? recordedRequest = null;
    
        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(true);
    
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes(x => recordedRequest = x.GetArgument<Request>(0)!);
    
        var request = new RequestLog
        {
            Bytes = long.MaxValue,
            CacheHits = long.MaxValue,
            CacheTTL = long.MaxValue,
            Costs = double.MaxValue,
            ElapsedMs = double.MaxValue,
            StatusCode = int.MaxValue,
            Timestamp = default
        };
    
        await sut.LogAsync(AppId.Id, request, CancellationToken.None);
    
        Assert.NotNull(recordedRequest);
    
        Assert.Contains(DefaultAppLogStore.FieldBytes, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldCacheHits, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldCacheTTL, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldCosts, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldRequestElapsedMs, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldStatusCode, recordedRequest.Properties);
    }

*/
    private static Request CreateRecord()
/*
FAILED TEST: The test run failed due to **C# syntax errors** in the file `DefaultAppLogStoreTests.cs`, preventing successful compilation:

### Errors:
1. **Line 148**: Missing semicolon (`;`) at the end of a statement.
2. **Lines 393–396**: Invalid token `{` and incorrect code block placement, indicating a malformed method or class member.

### Recommended Fixes:
1. Add a missing semicolon at the end of the line indicated in the error (`line 148`).
2. Correct the syntax at `line 393` and following lines by ensuring:
   - Correct placement of braces `{}`.
   - Valid assignment or declaration syntax.

These fixes will resolve the compilation errors and allow the test to run.

    [Fact]
    public async Task Should_add_max_values_to_request()
    {
        Request? recordedRequest = null;
    
        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(true);
    
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes(x => recordedRequest = x.GetArgument<Request>(0)!);
    
        var request = new RequestLog
        {
            Bytes = long.MaxValue,
            CacheHits = long.MaxValue,
            CacheTTL = long.MaxValue,
            Costs = double.MaxValue,
            ElapsedMs = double.MaxValue,
            StatusCode = int.MaxValue,
            Timestamp = default
        };
    
        await sut.LogAsync(AppId.Id, request, CancellationToken.None);
    
        Assert.NotNull(recordedRequest);
    
        Assert.Contains(DefaultAppLogStore.FieldBytes, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldCacheHits, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldCacheTTL, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldCosts, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldRequestElapsedMs, recordedRequest.Properties);
        Assert.Contains(DefaultAppLogStore.FieldStatusCode, recordedRequest.Properties);
    }

*/
/*
FAILED TEST: The test run failed due to **C# syntax errors** in the file `DefaultAppLogStoreTests.cs`:

1. **Line 152**: A semicolon (`;`) is missing at the end of a statement.
2. **Lines 350–353**: There is an invalid token `{` and incorrect code block placement, indicating a malformed method or class member.

### Recommended Fixes:
1. Add a missing semicolon at the end of the line indicated in the error (`line 152`).
2. Correct the syntax at `line 350` and following lines by ensuring:
   - Correct placement of braces `{}`.
   - Valid assignment or declaration syntax.

    [Fact]
    public async Task Should_return_null_for_invalid_data_in_GetDouble_and_GetLong()
    {
        var timeFrom = SystemClock.Instance.GetCurrentInstant();
        var timeTo = timeFrom.Plus(Duration.FromDays(4));
    
        var request = new Request
        {
            Properties = new Dictionary<string, string>
            {
                { FieldRequestElapsedMs, "not a number" },
                { FieldCosts, "invalid" },
                { FieldBytes, "abc" },
                { FieldCacheHits, "xyz" },
                { FieldCacheTTL, "123.45" },
                { FieldStatusCode, "not a long" }
            }
        };
    
        A.CallTo(() => requestLogStore.QueryAllAsync(AppId.Id.ToString(), timeFrom, timeTo, CancellationToken.None))
            .Returns(new[] { request }.ToAsyncEnumerable());
    
        var stream = new MemoryStream();
    
        await sut.ReadLogAsync(AppId.Id, timeFrom, timeTo, stream, CancellationToken.None);
        stream.Position = 0;
    
        using (var reader = new StreamReader(stream))
        {
            var header = await reader.ReadLineAsync();
            var dataLine = await reader.ReadLineAsync();
    
            Assert.NotNull(header);
            Assert.NotNull(dataLine);
        }
    }

*/
/*
FAILED TEST: The test run failed due to **C# syntax errors** in the file `DefaultAppLogStoreTests.cs`:

1. **Line 154**: A semicolon (`;`) is missing at the end of a statement.
2. **Lines 300–303**: There is an invalid token `{` and incorrect code block placement, indicating a malformed method or class member.

### Recommended Fixes:
1. Add a missing semicolon at the end of the line indicated in the error (`line 154`).
2. Correct the syntax at `line 300` and following lines by ensuring:
   - Correct placement of braces `{}`.
   - Valid assignment or declaration syntax.

These fixes will resolve the compilation errors and allow the test to run.

    [Fact]
    public async Task Should_write_correct_number_of_records_to_stream()
    {
        var timeFrom = SystemClock.Instance.GetCurrentInstant();
        var timeTo = timeFrom.Plus(Duration.FromDays(4));
    
        A.CallTo(() => requestLogStore.QueryAllAsync(AppId.Id.ToString(), timeFrom, timeTo, CancellationToken))
            .Returns(Enumerable.Empty<Request>().ToAsyncEnumerable());
    
        var stream = new MemoryStream();
    
        await sut.ReadLogAsync(AppId.Id, timeFrom, timeTo, stream, CancellationToken);
        stream.Position = 0;
    
        var lines = 0;
        using (var reader = new StreamReader(stream))
        {
            while (await reader.ReadLineAsync() != null)
            {
                lines++;
            }
        }
    
        Assert.Equal(1, lines); // Only the header line
    
        A.CallTo(() => requestLogStore.QueryAllAsync(AppId.Id.ToString(), timeFrom, timeTo, CancellationToken))
            .Returns(new[] { CreateRecord() }.ToAsyncEnumerable());
    
        stream = new MemoryStream();
    
        await sut.ReadLogAsync(AppId.Id, timeFrom, timeTo, stream, CancellationToken);
        stream.Position = 0;
    
        lines = 0;
        using (var reader = new StreamReader(stream))
        {
            while (await reader.ReadLineAsync() != null)
            {
                lines++;
            }
        }
    
        Assert.Equal(2, lines); // Header + one data line
    }

*/
/*
FAILED TEST: The test run failed due to **C# syntax errors** in the test file `DefaultAppLogStoreTests.cs`:

1. **Line 147**: A semicolon (`;`) is missing at the end of a statement.
2. **Lines 183–186**: There is an invalid token `{` and incorrect code block placement, indicating a malformed method or class member.

### Recommended Fixes:
1. Add a missing semicolon at the end of the line indicated in the error (`line 147`).
2. Correct the syntax at `line 183` and following lines by ensuring:
   - Correct placement of braces `{}`.
   - Valid assignment or declaration syntax.

These fixes will resolve the compilation errors and allow the test to run.

    [Fact]
    public async Task Should_not_add_non_numeric_values_to_request()
    {
        Request? recordedRequest = null;
    
        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(true);
    
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes(x => recordedRequest = x.GetArgument<Request>(0)!);
    
        var request = new RequestLog
        {
            Bytes = "not a number",
            CacheHits = "invalid",
            CacheTTL = "abc",
            Costs = "xyz",
            ElapsedMs = "120.5",
            StatusCode = "not a long",
            Timestamp = default
        };
    
        await sut.LogAsync(AppId.Id, request, CancellationToken.None);
    
        Assert.NotNull(recordedRequest);
    
        Assert.DoesNotContain(FieldBytes, recordedRequest.Properties);
        Assert.DoesNotContain(FieldCacheHits, recordedRequest.Properties);
        Assert.DoesNotContain(FieldCacheTTL, recordedRequest.Properties);
        Assert.DoesNotContain(FieldCosts, recordedRequest.Properties);
        Assert.DoesNotContain(FieldRequestElapsedMs, recordedRequest.Properties);
        Assert.DoesNotContain(FieldStatusCode, recordedRequest.Properties);
    }

*/
/*
FAILED TEST: The test run failed due to **C# syntax errors** in the test file `DefaultAppLogStoreTests.cs`. The compiler errors indicate malformed code structure, specifically:

- **Line 147**: Missing semicolon (`;`) at the end of a statement.
- **Lines 183–186**: Invalid token `{` and incorrect placement of code block, suggesting a misplaced or improperly structured method or class member.

### Recommended Fixes:
1. Add a missing semicolon at the end of the line indicated in the error (`line 147`).
2. Correct the syntax at `line 183` and following lines by ensuring:
   - Proper method or class member declaration.
   - Correct placement of braces `{}`.
   - Valid assignment or declaration syntax.

These fixes will resolve the compilation errors and allow the test to run.

    [Fact]
    public async Task Should_not_add_null_or_empty_string_fields_to_request()
    {
        Request? recordedRequest = null;
    
        A.CallTo(() => requestLogStore.IsEnabled)
            .Returns(true);
    
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._))
            .Invokes(x => recordedRequest = x.GetArgument<Request>(0)!);
    
        var request = new RequestLog
        {
            UserClientId = null,
            UserId = string.Empty,
            RequestPath = null,
            RequestMethod = null,
            CacheServer = null,
            CacheStatus = string.Empty,
            Timestamp = default
        };
    
        await sut.LogAsync(AppId.Id, request, CancellationToken.None);
    
        Assert.NotNull(recordedRequest);
    
        Assert.DoesNotContain(FieldAuthClientId, recordedRequest.Properties);
        Assert.DoesNotContain(FieldAuthUserId, recordedRequest.Properties);
        Assert.DoesNotContain(FieldRequestPath, recordedRequest.Properties);
        Assert.DoesNotContain(FieldRequestMethod, recordedRequest.Properties);
        Assert.DoesNotContain(FieldCacheServer, recordedRequest.Properties);
        Assert.DoesNotContain(FieldCacheStatus, recordedRequest.Properties);
    }

*/
    {
        return new Request { Properties = [] };
    }
}
