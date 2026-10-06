using Menchul.Import.GeoNames.org.IntegrationTests.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal sealed class EfDatabaseManager
{
    private readonly ProcessRunner __runner;
    private readonly ILogger __logger;

    public EfDatabaseManager(ProcessRunner runner, ILogger logger)
    {
        __runner = runner;
        __logger = logger;
    }

    public async Task<bool> DropDatabaseAsync(string projectPath, string connectionString, string workingDirectory)
    {
        string arguments = $"ef database drop --force --project \"{projectPath}\" --startup-project \"{projectPath}\" --framework {IntegrationTestConstants.EfCli.TargetFramework} -- --connection \"{connectionString}\"";
        ProcessResult result = await __runner.RunAsync(IntegrationTestConstants.EfCli.DotnetCommand, arguments, workingDirectory, 300);

        return result.ExitCode == 0;
    }

    public async Task<bool> UpdateDatabaseAsync(string projectPath, string connectionString, string workingDirectory)
    {
        string arguments = $"ef database update --project \"{projectPath}\" --startup-project \"{projectPath}\" --framework {IntegrationTestConstants.EfCli.TargetFramework} -- --connection \"{connectionString}\"";
        ProcessResult result = await __runner.RunAsync(IntegrationTestConstants.EfCli.DotnetCommand, arguments, workingDirectory, 300);

        return result.ExitCode == 0;
    }

    public async Task<bool> RecreateDatabaseAsync(string databaseName, string projectPath, string connectionString, string workingDirectory)
    {
        string logDrop = $"Dropping database '{databaseName}' via 'dotnet ef database drop'...";
        __logger.LogInformation(logDrop);
        bool dropOk = await DropDatabaseAsync(projectPath, connectionString, workingDirectory);

        if (!dropOk)
        {
            string dropError = $"Failed to drop database '{databaseName}' via dotnet ef.";
            __logger.LogError(dropError);

            return false;
        }

        string logUpdate = $"Creating/updating database '{databaseName}' via 'dotnet ef database update'...";
        __logger.LogInformation(logUpdate);
        bool updateOk = await UpdateDatabaseAsync(projectPath, connectionString, workingDirectory);

        if (!updateOk)
        {
            string updateError = $"Failed to update database '{databaseName}' via dotnet ef.";
            __logger.LogError(updateError);

            return false;
        }

        string logSuccess = $"Database '{databaseName}' successfully recreated via EF tools.";
        __logger.LogInformation(logSuccess);

        return true;
    }
}