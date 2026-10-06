using Menchul.Import.GeoNames.org.IntegrationTests.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal sealed class IntegrationTestReporter
{
    private readonly ILogger __logger;

    public IntegrationTestReporter(ILogger logger)
    {
        __logger = logger;
    }

    public string GenerateReport(bool dockerAvailable, IReadOnlyList<FileInspectionResult> fileResults, IReadOnlyList<DatabaseInspectionResult> dbResults)
    {
        var builder = new StringBuilder();
        builder.AppendLine(IntegrationTestConstants.Report.HeaderSeparator);
        builder.AppendLine(IntegrationTestConstants.Report.Title);
        builder.AppendLine(IntegrationTestConstants.Report.HeaderSeparator);

        string dockerStatus = dockerAvailable ? IntegrationTestConstants.Report.DockerAvailable : IntegrationTestConstants.Report.DockerNotAvailable;
        string dockerLine = $"Docker Engine: {dockerStatus}";
        builder.AppendLine(dockerLine);
        builder.AppendLine();

        builder.AppendLine(IntegrationTestConstants.Report.SectionSeparator);
        builder.AppendLine(IntegrationTestConstants.Report.Section1Title);
        builder.AppendLine(IntegrationTestConstants.Report.SectionSeparator);

        foreach (FileInspectionResult file in fileResults)
        {
            double sizeMb = (double)file.FileSizeBytes / (1024 * 1024);
            string fileSummary = $"  - {file.FileName,-25} | Size: {sizeMb,8:F2} MB | Lines: {file.LineCount,12:N0}";
            builder.AppendLine(fileSummary);
        }

        builder.AppendLine();
        builder.AppendLine(IntegrationTestConstants.Report.SectionSeparator);
        builder.AppendLine(IntegrationTestConstants.Report.Section2Title);
        builder.AppendLine(IntegrationTestConstants.Report.SectionSeparator);

        foreach (DatabaseInspectionResult db in dbResults)
        {
            string durationStr = $"{db.ImportDuration:hh\\:mm\\:ss}";
            string statusStr = db.IsSuccess ? IntegrationTestConstants.Report.StatusPassed : IntegrationTestConstants.Report.StatusFailed;
            string dbHeader = $"Database: {db.ServerName,-12} | Duration: {durationStr} | Status: {statusStr}";
            builder.AppendLine(dbHeader);

            foreach (TableCountResult table in db.TableCounts)
            {
                string tableLine = $"    * {table.TableName,-20}: {table.RowCount,12:N0} rows";
                builder.AppendLine(tableLine);
            }

            builder.AppendLine();
        }

        builder.AppendLine(IntegrationTestConstants.Report.HeaderSeparator);
        string reportText = builder.ToString();
        __logger.LogInformation(reportText);

        return reportText;
    }
}