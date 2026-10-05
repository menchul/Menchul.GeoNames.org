namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal static class IntegrationTestConstants
{
    public static class DatabaseServers
    {
        public const string SQLite = nameof(Server.SQLite);
        public const string PostgreSQL = nameof(Server.PostgreSQL);
        public const string MSSQL = nameof(Server.MSSQL);
    }

    public static class ConnectionStrings
    {
        public const string PostgreSql = "Host=localhost;Port=5433;Database=GeoNames.org;Username=postgres;Password=1qaz@WSX;";
        public const string MsSql = "Server=localhost,1433;Database=GeoNames.org;User Id=sa;Password=1qaz@WSX;TrustServerCertificate=True;";
        public const string SqLiteDatabaseFileName = "GeoNames.org.db";
    }

    public static class Docker
    {
        public const string Command = "docker";
        public const string Info = "info";
        public const string PostgreSqlContainerName = "GeoNamesOrg_PSQL";
        public const string MsSqlContainerName = "GeoNamesOrg_ms_sql_2022_latest";
        public const string Password = "1qaz@WSX";
        public const string DatabaseName = "GeoNames.org";
        public const string PostgreSqlImage = "postgres:16";
        public const string MsSqlImage = "mcr.microsoft.com/mssql/server:2022-latest";
        public const string PostgreSqlVolume = "geonamesorg_pgdata:/var/lib/postgresql/data";
        public const string PostgreSqlPortMapping = "5433:5432";
        public const string MsSqlPortMapping = "1433:1433";
        public const string SqlCmdPath = "/opt/mssql-tools18/bin/sqlcmd";
        public const string SqlCmdHost = "localhost";
        public const string MsSqlSaUser = "sa";
        public const string PostgreSqlUser = "postgres";
        public const string PgIsReadyCommand = "pg_isready -U postgres";
        public const string PingSqlQuery = "SELECT 1;";
        public const string CreateDatabaseSqlQuery = "IF DB_ID('GeoNames.org') IS NULL CREATE DATABASE [GeoNames.org];";
    }

    public static class EfCli
    {
        public const string DotnetCommand = "dotnet";
        public const string TargetFramework = "net10.0";
    }

    public static class Paths
    {
        public const string TempFolder = ".tmp";
        public const string EditorConfigFile = ".editorconfig";
        public const string ReleaseConfiguration = "Release";
        public const string ZipExtension = ".zip";
        public const string TextExtension = ".txt";
    }

    public static class StepMessages
    {
        public const string SuiteStart = "Starting GeoNames Integration Test Suite...";
        public const string Step1DockerCheck = "Step 1: Checking Docker installation and status...";
        public const string Step2ContainersStart = "Step 2: Starting database containers in Docker...";
        public const string Step3And4DownloadFiles = "Step 3 & 4: Downloading dump files and verifying presence...";
        public const string Step5LineCounts = "Step 5: File line counts determined successfully.";
        public const string Step6RecreateSQLite = "Step 6: Recreating SQLite database via EF tools and running importer...";
        public const string Step6RecreatePostgreSql = "Step 6: Recreating PostgreSQL database via EF tools and running importer...";
        public const string Step6RecreateMsSql = "Step 6: Recreating MSSQL database via EF tools and running importer...";
        public const string Step7ValidateRowCounts = "Step 7: Validating row counts across databases...";
        public const string Step8CleanupContainers = "Step 8: Cleaning up containers (Docker engine remains untouched)...";
        public const string Step9GenerateReport = "Step 9: Generating final report...";
    }

    public static class Report
    {
        public const string HeaderSeparator = "================================================================================";
        public const string SectionSeparator = "--------------------------------------------------------------------------------";
        public const string Title = "                      GEONAMES INTEGRATION TEST REPORT                          ";
        public const string Section1Title = "1. DUMP FILES DOWNLOAD & LINE COUNTS";
        public const string Section2Title = "2. DATABASE IMPORT DURATION & RECORD COUNTS";
        public const string DockerAvailable = "AVAILABLE & RUNNING";
        public const string DockerNotAvailable = "NOT AVAILABLE";
        public const string StatusPassed = "PASSED";
        public const string StatusFailed = "FAILED";
    }
}