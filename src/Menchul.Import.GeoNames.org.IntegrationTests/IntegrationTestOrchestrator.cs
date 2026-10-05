using Menchul.Import.GeoNames.org.IntegrationTests.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal sealed class IntegrationTestOrchestrator
{
    private const string __sqLiteServerName = IntegrationTestConstants.DatabaseServers.SQLite;
    private const string __postgreSqlServerName = IntegrationTestConstants.DatabaseServers.PostgreSQL;
    private const string __msSqlServerName = IntegrationTestConstants.DatabaseServers.MSSQL;

    private const string __postgreSqlConnectionString = IntegrationTestConstants.ConnectionStrings.PostgreSql;
    private const string __msSqlConnectionString = IntegrationTestConstants.ConnectionStrings.MsSql;

    private readonly ILogger __logger;
    private readonly DockerManager __dockerManager;
    private readonly EfDatabaseManager __efDatabaseManager;
    private readonly FileDownloadManager __fileManager;
    private readonly ImporterRunner __importerRunner;
    private readonly DatabaseVerifier __databaseVerifier;
    private readonly IntegrationTestReporter __reporter;

    public IntegrationTestOrchestrator(ILogger logger)
    {
        __logger = logger;
        var runner = new ProcessRunner(logger);
        __dockerManager = new DockerManager(runner, logger);
        __efDatabaseManager = new EfDatabaseManager(runner, logger);
        __fileManager = new FileDownloadManager(logger);
        __importerRunner = new ImporterRunner(runner, logger);
        __databaseVerifier = new DatabaseVerifier(logger);
        __reporter = new IntegrationTestReporter(logger);
    }

    public async Task<bool> RunAllAsync(bool cleanupContainers = true)
    {
        __logger.LogInformation(IntegrationTestConstants.StepMessages.SuiteStart);

        string repoRoot = FindRepositoryRoot();
        string tempFolder = Path.Combine(repoRoot, IntegrationTestConstants.Paths.TempFolder);
        string importerProjectPath = Path.Combine(repoRoot, "src", "Menchul.Import.GeoNames.org", "Menchul.Import.GeoNames.org.csproj");
        string sqliteProjectPath = Path.Combine(repoRoot, "src", "Menchul.GeoNames.org.SQLite", "Menchul.GeoNames.org.SQLite.csproj");
        string psqlProjectPath = Path.Combine(repoRoot, "src", "Menchul.GeoNames.org.PostgreSQL", "Menchul.GeoNames.org.PostgreSQL.csproj");
        string mssqlProjectPath = Path.Combine(repoRoot, "src", "Menchul.GeoNames.org.MSSQL", "Menchul.GeoNames.org.MSSQL.csproj");
        string sqliteDbPath = Path.Combine(tempFolder, IntegrationTestConstants.ConnectionStrings.SqLiteDatabaseFileName);
        string sqliteConnectionString = $"Data Source={sqliteDbPath};";

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step1DockerCheck);
        bool isDockerRunning = await __dockerManager.CheckDockerInstalledAndRunningAsync();

        if (!isDockerRunning)
        {
            __logger.LogError("Docker check failed. Terminating test suite.");

            return false;
        }

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step2ContainersStart);
        bool psqlReady = await __dockerManager.EnsurePostgreSqlRunningAsync();

        if (!psqlReady)
        {
            __logger.LogError("PostgreSQL container failed to become ready.");

            return false;
        }

        bool mssqlReady = await __dockerManager.EnsureMsSqlRunningAsync();

        if (!mssqlReady)
        {
            __logger.LogError("MSSQL container failed to become ready.");

            return false;
        }

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step3And4DownloadFiles);
        IReadOnlyList<FileInspectionResult> fileResults = await __fileManager.DownloadAndInspectFilesAsync(tempFolder);

        foreach (FileInspectionResult file in fileResults)
        {
            bool exists = file.Exists;
            long size = file.FileSizeBytes;

            if (!exists || size == 0)
            {
                string error = $"Dump file missing or empty: {file.FileName}";
                __logger.LogError(error);

                return false;
            }
        }

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step5LineCounts);

        var dbResults = new List<DatabaseInspectionResult>();

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step6RecreateSQLite);
        bool sqliteRecreated = await __efDatabaseManager.RecreateDatabaseAsync(__sqLiteServerName, sqliteProjectPath, sqliteConnectionString, repoRoot);

        if (!sqliteRecreated)
        {
            __logger.LogError("SQLite database drop/update failed.");

            return false;
        }

        TimeSpan sqliteDuration = await __importerRunner.RunImportAsync(importerProjectPath, __sqLiteServerName, sqliteConnectionString, tempFolder);
        IReadOnlyList<TableCountResult> sqliteCounts = await __databaseVerifier.VerifySqLiteAsync(sqliteConnectionString);
        var sqliteResult = new DatabaseInspectionResult(__sqLiteServerName, sqliteDuration, sqliteCounts, true);
        dbResults.Add(sqliteResult);

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step6RecreatePostgreSql);
        bool psqlRecreated = await __efDatabaseManager.RecreateDatabaseAsync(__postgreSqlServerName, psqlProjectPath, __postgreSqlConnectionString, repoRoot);

        if (!psqlRecreated)
        {
            __logger.LogError("PostgreSQL database drop/update failed.");

            return false;
        }

        TimeSpan psqlDuration = await __importerRunner.RunImportAsync(importerProjectPath, __postgreSqlServerName, __postgreSqlConnectionString, tempFolder);
        IReadOnlyList<TableCountResult> psqlCounts = await __databaseVerifier.VerifyPostgreSqlAsync(__postgreSqlConnectionString);
        var psqlResult = new DatabaseInspectionResult(__postgreSqlServerName, psqlDuration, psqlCounts, true);
        dbResults.Add(psqlResult);

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step6RecreateMsSql);
        bool mssqlRecreated = await __efDatabaseManager.RecreateDatabaseAsync(__msSqlServerName, mssqlProjectPath, __msSqlConnectionString, repoRoot);

        if (!mssqlRecreated)
        {
            __logger.LogError("MSSQL database drop/update failed.");

            return false;
        }

        TimeSpan mssqlDuration = await __importerRunner.RunImportAsync(importerProjectPath, __msSqlServerName, __msSqlConnectionString, tempFolder);
        IReadOnlyList<TableCountResult> mssqlCounts = await __databaseVerifier.VerifyMsSqlAsync(__msSqlConnectionString);
        var mssqlResult = new DatabaseInspectionResult(__msSqlServerName, mssqlDuration, mssqlCounts, true);
        dbResults.Add(mssqlResult);

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step7ValidateRowCounts);
        bool countsValid = ValidateRowCounts(sqliteCounts, psqlCounts, mssqlCounts);

        if (!countsValid)
        {
            __logger.LogError("Row counts validation failed across databases.");
        }

        if (cleanupContainers)
        {
            __logger.LogInformation(IntegrationTestConstants.StepMessages.Step8CleanupContainers);
            await __dockerManager.CleanupContainersAsync();
        }

        __logger.LogInformation(IntegrationTestConstants.StepMessages.Step9GenerateReport);
        __reporter.GenerateReport(isDockerRunning, fileResults, dbResults);

        return countsValid;
    }

    private static string FindRepositoryRoot()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var directory = new DirectoryInfo(baseDir);

        while (directory != null)
        {
            string editorConfigPath = Path.Combine(directory.FullName, IntegrationTestConstants.Paths.EditorConfigFile);
            bool hasEditorConfig = File.Exists(editorConfigPath);

            if (hasEditorConfig)
            {
                string root = directory.FullName;

                return root;
            }

            directory = directory.Parent;
        }

        string fallback = Directory.GetCurrentDirectory();

        return fallback;
    }

    private bool ValidateRowCounts(IReadOnlyList<TableCountResult> sqlite, IReadOnlyList<TableCountResult> psql, IReadOnlyList<TableCountResult> mssql)
    {
        int count1 = sqlite.Count;
        int count2 = psql.Count;
        int count3 = mssql.Count;

        if (count1 == 0 || count2 == 0 || count3 == 0)
        {
            __logger.LogError("One or more databases returned zero tables.");

            return false;
        }

        for (int i = 0; i < count1; i++)
        {
            TableCountResult s = sqlite[i];
            long sCount = s.RowCount;

            if (sCount == 0)
            {
                string emptyTableMsg = $"Table {s.TableName} is empty in SQLite";
                __logger.LogError(emptyTableMsg);

                return false;
            }
        }

        __logger.LogInformation("All database table counts are positive and validated.");

        return true;
    }
}