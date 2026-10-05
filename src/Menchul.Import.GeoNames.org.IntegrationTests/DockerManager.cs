using Menchul.Import.GeoNames.org.IntegrationTests.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal sealed class DockerManager
{
    private const string __postgreSqlContainerName = IntegrationTestConstants.Docker.PostgreSqlContainerName;
    private const string __msSqlContainerName = IntegrationTestConstants.Docker.MsSqlContainerName;

    private readonly ProcessRunner __runner;
    private readonly ILogger __logger;

    public DockerManager(ProcessRunner runner, ILogger logger)
    {
        __runner = runner;
        __logger = logger;
    }

    public async Task<bool> CheckDockerInstalledAndRunningAsync()
    {
        __logger.LogInformation("Verifying Docker installation and daemon status...");
        ProcessResult infoResult = await __runner.RunAsync(IntegrationTestConstants.Docker.Command, IntegrationTestConstants.Docker.Info, null, 30);
        int exitCode = infoResult.ExitCode;
        bool isRunning = exitCode == 0;

        if (isRunning)
        {
            __logger.LogInformation("Docker is installed and running.");

            return true;
        }

        __logger.LogError("Docker daemon is not reachable or not installed.");

        return false;
    }

    public async Task<bool> EnsurePostgreSqlRunningAsync()
    {
        __logger.LogInformation("Ensuring PostgreSQL container is up and running...");
        string inspectArgs = $"ps -q -f name=^/{__postgreSqlContainerName}$";
        ProcessResult runningCheck = await __runner.RunAsync(IntegrationTestConstants.Docker.Command, inspectArgs, null, 30);
        string runningOutput = runningCheck.StandardOutput.Trim();
        bool isRunning = !string.IsNullOrWhiteSpace(runningOutput);

        if (!isRunning)
        {
            string allCheckArgs = $"ps -a -q -f name=^/{__postgreSqlContainerName}$";
            ProcessResult existingCheck = await __runner.RunAsync(IntegrationTestConstants.Docker.Command, allCheckArgs, null, 30);
            string existingOutput = existingCheck.StandardOutput.Trim();
            bool exists = !string.IsNullOrWhiteSpace(existingOutput);

            if (exists)
            {
                string startArgs = $"start {__postgreSqlContainerName}";
                await __runner.RunAsync(IntegrationTestConstants.Docker.Command, startArgs, null, 60);
            }
            else
            {
                string runArgs = $"run -d --name {__postgreSqlContainerName} -e \"POSTGRES_PASSWORD={IntegrationTestConstants.Docker.Password}\" -e \"POSTGRES_DB={IntegrationTestConstants.Docker.DatabaseName}\" -p {IntegrationTestConstants.Docker.PostgreSqlPortMapping} -v {IntegrationTestConstants.Docker.PostgreSqlVolume} {IntegrationTestConstants.Docker.PostgreSqlImage}";
                await __runner.RunAsync(IntegrationTestConstants.Docker.Command, runArgs, null, 120);
            }
        }

        bool isReady = await WaitForPostgreSqlReadyAsync();

        return isReady;
    }

    public async Task<bool> EnsureMsSqlRunningAsync()
    {
        __logger.LogInformation("Ensuring MSSQL container is up and running...");
        string inspectArgs = $"ps -q -f name=^/{__msSqlContainerName}$";
        ProcessResult runningCheck = await __runner.RunAsync(IntegrationTestConstants.Docker.Command, inspectArgs, null, 30);
        string runningOutput = runningCheck.StandardOutput.Trim();
        bool isRunning = !string.IsNullOrWhiteSpace(runningOutput);

        if (!isRunning)
        {
            string allCheckArgs = $"ps -a -q -f name=^/{__msSqlContainerName}$";
            ProcessResult existingCheck = await __runner.RunAsync(IntegrationTestConstants.Docker.Command, allCheckArgs, null, 30);
            string existingOutput = existingCheck.StandardOutput.Trim();
            bool exists = !string.IsNullOrWhiteSpace(existingOutput);

            if (exists)
            {
                string startArgs = $"start {__msSqlContainerName}";
                await __runner.RunAsync(IntegrationTestConstants.Docker.Command, startArgs, null, 60);
            }
            else
            {
                string runArgs = $"run -d --name {__msSqlContainerName} -e \"ACCEPT_EULA=Y\" -e \"MSSQL_SA_PASSWORD={IntegrationTestConstants.Docker.Password}\" -p {IntegrationTestConstants.Docker.MsSqlPortMapping} {IntegrationTestConstants.Docker.MsSqlImage}";
                await __runner.RunAsync(IntegrationTestConstants.Docker.Command, runArgs, null, 120);
            }
        }

        bool isReady = await WaitForMsSqlReadyAsync();

        if (isReady)
        {
            string createDbQuery = IntegrationTestConstants.Docker.CreateDatabaseSqlQuery;
            string createDbArgs = $"exec -i {__msSqlContainerName} {IntegrationTestConstants.Docker.SqlCmdPath} -S {IntegrationTestConstants.Docker.SqlCmdHost} -C -U {IntegrationTestConstants.Docker.MsSqlSaUser} -P {IntegrationTestConstants.Docker.Password} -Q \"{createDbQuery}\"";
            await __runner.RunAsync(IntegrationTestConstants.Docker.Command, createDbArgs, null, 60);
        }

        return isReady;
    }

    public async Task CleanupContainersAsync()
    {
        __logger.LogInformation("Cleaning up database containers (Docker engine remains untouched)...");
        string stopPsqlArgs = $"stop {__postgreSqlContainerName}";
        await __runner.RunAsync(IntegrationTestConstants.Docker.Command, stopPsqlArgs, null, 60);

        string stopMssqlArgs = $"stop {__msSqlContainerName}";
        await __runner.RunAsync(IntegrationTestConstants.Docker.Command, stopMssqlArgs, null, 60);

        __logger.LogInformation("Database containers stopped successfully.");
    }

    private async Task<bool> WaitForPostgreSqlReadyAsync()
    {
        const int maxAttempts = 30;
        string readyArgs = $"exec {__postgreSqlContainerName} {IntegrationTestConstants.Docker.PgIsReadyCommand}";

        for (int i = 1; i <= maxAttempts; i++)
        {
            ProcessResult readyResult = await __runner.RunAsync(IntegrationTestConstants.Docker.Command, readyArgs, null, 10);
            int exitCode = readyResult.ExitCode;

            if (exitCode == 0)
            {
                __logger.LogInformation("PostgreSQL is ready to accept connections.");

                return true;
            }

            const int delayMilliseconds = 1000;
            await Task.Delay(delayMilliseconds);
        }

        __logger.LogError("PostgreSQL did not become ready within the timeout period.");

        return false;
    }

    private async Task<bool> WaitForMsSqlReadyAsync()
    {
        const int maxAttempts = 30;
        string pingArgs = $"exec {__msSqlContainerName} {IntegrationTestConstants.Docker.SqlCmdPath} -S {IntegrationTestConstants.Docker.SqlCmdHost} -C -U {IntegrationTestConstants.Docker.MsSqlSaUser} -P {IntegrationTestConstants.Docker.Password} -Q \"{IntegrationTestConstants.Docker.PingSqlQuery}\"";

        for (int i = 1; i <= maxAttempts; i++)
        {
            ProcessResult readyResult = await __runner.RunAsync(IntegrationTestConstants.Docker.Command, pingArgs, null, 10);
            int exitCode = readyResult.ExitCode;

            if (exitCode == 0)
            {
                __logger.LogInformation("MSSQL is ready to accept connections.");

                return true;
            }

            const int delayMilliseconds = 1500;
            await Task.Delay(delayMilliseconds);
        }

        __logger.LogError("MSSQL did not become ready within the timeout period.");

        return false;
    }
}