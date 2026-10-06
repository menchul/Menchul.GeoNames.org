using Menchul.Import.GeoNames.org.IntegrationTests.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal sealed class ImporterRunner
{
    private readonly ProcessRunner __runner;
    private readonly ILogger __logger;

    public ImporterRunner(ProcessRunner runner, ILogger logger)
    {
        __runner = runner;
        __logger = logger;
    }

    public async Task<TimeSpan> RunImportAsync(string projectPath, string serverName, string connectionString, string tempFolder)
    {
        string startLog = $"Starting import for {serverName}...";
        __logger.LogInformation(startLog);

        string arguments = $"run -c {IntegrationTestConstants.Paths.ReleaseConfiguration} --project \"{projectPath}\" -- {CommandLineConstants.Server} {serverName} {CommandLineConstants.ConnectionString} \"{connectionString}\" {CommandLineConstants.TempFolder} \"{tempFolder}\" {CommandLineConstants.KeepTempFiles} {CommandLineConstants.ImportOnlyAP} {CommandLineConstants.NormalizeData}";

        var stopwatch = Stopwatch.StartNew();
        ProcessResult processResult = await __runner.RunAsync(IntegrationTestConstants.EfCli.DotnetCommand, arguments, null, 1800);
        stopwatch.Stop();

        int exitCode = processResult.ExitCode;

        if (exitCode != 0)
        {
            string errorMessage = $"Import failed for {serverName} with exit code {exitCode}";
            __logger.LogError(errorMessage);

            throw new InvalidOperationException(errorMessage);
        }

        TimeSpan duration = stopwatch.Elapsed;
        string endLog = $"Import for {serverName} finished successfully in {duration:hh\\:mm\\:ss}.";
        __logger.LogInformation(endLog);

        return duration;
    }
}