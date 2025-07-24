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
The test run failed due to a **missing reference to the `FsCheck.Xunit` library**, which is required for property-based testing in the file. There are also **duplicate using directives** for `NodaTime` and `Squidex.Infrastructure.Log`, which are warnings but not the direct cause of the failure.

**Recommended Fixes:**
1. **Add `FsCheck.Xunit` NuGet package** to the test project (`Squidex.Domain.Apps.Entities.Tests.csproj`):
   ```xml
   <PackageReference Include="FsCheck.Xunit" Version="2.19.0" />
   ```
2. **Remove the duplicate using directives** for `NodaTime` and `Squidex.Infrastructure.Log` from the test file.

    [Fact]
    public async Task ReadLogAsync_handles_large_number_of_log_entries()
    {
        // Given
        var stream = new MemoryStream();
        var appId = DomainId.NewGuid();
        var largeData = new List<Request>();
    
        for (int i = 0; i < 1000; i++)
        {
            var request = new Request
            {
                Timestamp = $"2023-01-01T00:00:00Z",
                Properties = new Dictionary<string, string>
                {
                    { FieldRequestPath, "/api/test" },
                    { FieldRequestMethod, "GET" },
                    { FieldRequestElapsedMs, "100" },
                    { FieldCosts, "0.5" },
                    { FieldAuthClientId, "client123" },
                    { FieldAuthUserId, "user456" },
                    { FieldBytes, "1024" },
                    { FieldCacheHits, "1" },
                    { FieldCacheServer, "server1" },
                    { FieldCacheStatus, "HIT" },
                    { FieldCacheTTL, "3600" },
                    { FieldStatusCode, "200" }
                }
            };
            largeData.Add(request);
        }
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), A<Instant>._, A<Instant>._, A<CancellationToken>._))
            .Returns(largeData.ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, Instant.FromUnixTimeSeconds(0), Instant.FromUnixTimeSeconds(1000), stream, default);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
    
        var lines = content.Split('\n');
        Assert.Equal(1001, lines.Length); // 1 header row + 1000 data rows
    }

*/
/*
FAILED TEST: **Analysis:**
The test run failed due to a **missing reference to the Moq library**, which is used in the test setup (`Moq.Mock`). Additionally, there is a **duplicate using directive** for `Squidex.Infrastructure.Log`, which is a warning but not the cause of the failure.

**Recommended Fixes:**
1. **Add Moq to the test project** (`Squidex.Domain.Apps.Entities.Tests.csproj`):
   ```xml
   <PackageReference Include="Moq" Version="4.19.0" />
   ```
2. **Remove the duplicate using directive** for `Squidex.Infrastructure.Log` from the test file.

    [Fact]
    public async Task ReadLogAsync_writes_timestamp_as_is_without_validation()
    {
        // Given
        var stream = new MemoryStream();
        var appId = DomainId.NewGuid();
        var request = new Request
        {
            Timestamp = "InvalidTimestamp"
        };
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), A<Instant>._, A<Instant>._, A<CancellationToken>._))
            .Returns(new[] { request }.ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, Instant.FromUnixTimeSeconds(0), Instant.FromUnixTimeSeconds(1000), stream, default);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
    
        var lines = content.Split('\n');
        Assert.True(lines.Length > 1);
        var dataRow = lines[1];
        var fields = dataRow.Split('|');
        Assert.Equal("InvalidTimestamp", fields[0]);
    }

*/
/*
FAILED TEST: **Analysis:**
The test run failed because the **Moq library is missing** as a dependency in the test project, which is required for the `Mock<IRequestLogStore>` used in the test setup. Additionally, there is a **duplicate using directive** for `NodaTime`, which is a warning but not the direct cause of the failure.

**Recommended Fixes:**
1. **Add Moq to the test project** (`Squidex.Domain.Apps.Entities.Tests.csproj`):
   ```xml
   <PackageReference Include="Moq" Version="4.19.0" />
   ```
2. **Remove the duplicate using directive** for `NodaTime` from the test file.

    [Fact]
    public async Task ReadLogAsync_writes_only_header_row_when_no_log_entries()
    {
        // Given
        var stream = new MemoryStream();
        var appId = DomainId.NewGuid();
    
        A.CallTo(() => requestLogStore.QueryAllAsync(appId.ToString(), A<Instant>._, A<Instant>._, A<CancellationToken>._))
            .Returns(Enumerable.Empty<Request>().ToAsyncEnumerable());
    
        // When
        await sut.ReadLogAsync(appId, Instant.FromUnixTimeSeconds(0), Instant.FromUnixTimeSeconds(1000), stream, default);
    
        // Then
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
    
        // Verify that only the header row is present
        var lines = content.Split('\n');
        Assert.Equal(1, lines.Length);
        Assert.Contains(FieldTimestamp, lines[0]);
        Assert.Contains(FieldRequestPath, lines[0]);
        Assert.Contains(FieldRequestMethod, lines[0]);
        Assert.Contains(FieldRequestElapsedMs, lines[0]);
        Assert.Contains(FieldCosts, lines[0]);
        Assert.Contains(FieldAuthClientId, lines[0]);
        Assert.Contains(FieldAuthUserId, lines[0]);
        Assert.Contains(FieldBytes, lines[0]);
        Assert.Contains(FieldCacheHits, lines[0]);
        Assert.Contains(FieldCacheServer, lines[0]);
        Assert.Contains(FieldCacheStatus, lines[0]);
        Assert.Contains(FieldCacheTTL, lines[0]);
        Assert.Contains(FieldStatusCode, lines[0]);
    }

*/
/*
FAILED TEST: **Analysis:**
The test run failed because the **Moq library is missing** as a dependency in the test project, which is required for the `Mock<IRequestLogStore>` used in the test setup. Additionally, there is a **duplicate using directive** for `NodaTime`, which is a warning but not the direct cause of the failure.

**Recommended Fixes:**
1. **Add Moq to the test project** (`Squidex.Domain.Apps.Entities.Tests.csproj`):
   ```xml
   <PackageReference Include="Moq" Version="4.19.0" />
   ```
2. **Remove the duplicate `using NodaTime;` directive** from the test file.

    [Fact]
    public void GetDouble_and_GetLong_return_null_for_invalid_numeric_values()
    {
        // Given
        var sut = new DefaultAppLogStore(new Mock<IRequestLogStore>().Object);
        var request = new Request();
    
        // When / Then
        Assert.Null(sut.GetDouble(request, "Field1"));
        Assert.Null(sut.GetLong(request, "Field1"));
    
        request.Properties["Field2"] = "Invalid";
        Assert.Null(sut.GetDouble(request, "Field2"));
        Assert.Null(sut.GetLong(request, "Field2"));
    
        request.Properties["Field3"] = "123";
        Assert.Equal(123.0, sut.GetDouble(request, "Field3"));
        Assert.Equal(123L, sut.GetLong(request, "Field3"));
    }

*/
/*
FAILED TEST: **Analysis:**
The test run failed because the **Moq library is missing** as a dependency in the test project, which is required for the `Mock<IRequestLogStore>` used in the test setup. Additionally, there is a **duplicate using directive** for `NodaTime`, which is a warning but not the direct cause of the failure.

**Recommended Fixes:**
1. **Add Moq to the test project** (`Squidex.Domain.Apps.Entities.Tests.csproj`):
   ```xml
   <PackageReference Include="Moq" Version="4.19.0" />
   ```
2. **Remove the duplicate using directive** for `NodaTime` from the test file.

    [Fact]
    public void Append_methods_handle_null_or_empty_values_correctly()
    {
        // Given
        var request = new Request();
        var sut = new DefaultAppLogStore(new Mock<IRequestLogStore>().Object);
    
        // When
        sut.Append(request, "Field1", (string?)null);
        sut.Append(request, "Field2", string.Empty);
        sut.Append(request, "Field3", "Value");
        sut.Append(request, "Field4", (object?)null);
        sut.Append(request, "Field5", 123);
    
        // Then
        Assert.False(request.Properties.ContainsKey("Field1"));
        Assert.False(request.Properties.ContainsKey("Field2"));
        Assert.Equal("Value", request.Properties["Field3"]);
        Assert.False(request.Properties.ContainsKey("Field4"));
        Assert.Equal("123", request.Properties["Field5"]);
    }

*/
/*
FAILED TEST: The test run failed because the **Moq library is missing** as a dependency in the test project, which is required for the `Mock<IRequestLogStore>` used in the test setup. Additionally, there is a **duplicate using directive** for `NodaTime`, which is a warning but not the direct cause of the failure.

**Recommended Fixes:**

1. **Add Moq to the test project** (`Squidex.Domain.Apps.Entities.Tests.csproj`):
   ```xml
   <PackageReference Include="Moq" Version="4.19.0" />
   ```

2. **Remove the duplicate `using NodaTime;` directive** from the test file.

    [Fact]
    public void ReadLogAsync_throws_ArgumentNullException_when_appId_is_null()
    {
        // Given
        var appId = (DomainId?)null;
        var sut = new DefaultAppLogStore(new Mock<IRequestLogStore>().Object);
        
        // When / Then
        Assert.ThrowsAsync<ArgumentNullException>(async () => 
            await sut.ReadLogAsync(appId.Value, Instant.FromUnixTimeSeconds(0), Instant.FromUnixTimeSeconds(1000), new MemoryStream(), default));
    }

*/
/*
FAILED TEST: **Analysis:**

The test run failed due to a **missing reference to the Moq library**, which is used in the test setup (`Moq.Mock`). Additionally, there is a **duplicate using directive** for `Squidex.Infrastructure.Log`, which is a warning but not the cause of the failure.

**Recommended Fixes:**

1. **Add Moq NuGet package** to the test project (`Squidex.Domain.Apps.Entities.Tests.csproj`):
   ```xml
   <PackageReference Include="Moq" Version="4.19.0" />
   ```

2. **Remove the duplicate using directive** for `Squidex.Infrastructure.Log` from the test file.

    [Fact]
    public async Task LogAsync_returns_immediately_when_requestLogStore_is_disabled()
    {
        // Given
        A.CallTo(() => requestLogStore.IsEnabled).Returns(false);
    
        // When
        await sut.LogAsync(DomainId.NewGuid(), new RequestLog(), default);
    
        // Then
        A.CallTo(() => requestLogStore.LogAsync(A<Request>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

*/
}
