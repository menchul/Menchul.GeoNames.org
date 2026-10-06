namespace Menchul.Import.GeoNames.org.IntegrationTests.Models;

internal sealed class TableCountResult
{
    public string TableName { get; }

    public long RowCount { get; }

    public TableCountResult(string tableName, long rowCount)
    {
        TableName = tableName;
        RowCount = rowCount;
    }
}