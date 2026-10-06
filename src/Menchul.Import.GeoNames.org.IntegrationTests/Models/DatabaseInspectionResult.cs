using System;
using System.Collections.Generic;

namespace Menchul.Import.GeoNames.org.IntegrationTests.Models;

internal sealed class DatabaseInspectionResult
{
    public string ServerName { get; }

    public TimeSpan ImportDuration { get; }

    public IReadOnlyList<TableCountResult> TableCounts { get; }

    public bool IsSuccess { get; }

    public DatabaseInspectionResult(string serverName, TimeSpan importDuration, IReadOnlyList<TableCountResult> tableCounts, bool isSuccess)
    {
        ServerName = serverName;
        ImportDuration = importDuration;
        TableCounts = tableCounts;
        IsSuccess = isSuccess;
    }
}