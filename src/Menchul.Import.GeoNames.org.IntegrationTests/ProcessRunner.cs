using Menchul.Import.GeoNames.org.IntegrationTests.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal sealed class ProcessRunner
{
    private readonly ILogger __logger;

    public ProcessRunner(ILogger logger)
    {
        __logger = logger;
    }

    public async Task<ProcessResult> RunAsync(string fileName, string arguments, string? workingDirectory = null, int timeoutSeconds = 1200)
    {
        string logStart = $"Executing: {fileName} {arguments}";
        __logger.LogInformation(logStart);

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        using var process = new Process();
        process.StartInfo = startInfo;

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            string? data = e.Data;

            if (data != null)
            {
                outputBuilder.AppendLine(data);
                Console.WriteLine(data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            string? data = e.Data;

            if (data != null)
            {
                errorBuilder.AppendLine(data);
                Console.Error.WriteLine(data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        int timeoutMilliseconds = timeoutSeconds * 1000;
        using var cancellationTokenSource = new CancellationTokenSource(timeoutMilliseconds);
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            process.Kill(true);
            string timeoutError = $"Process timed out after {timeoutSeconds} seconds: {fileName}";
            __logger.LogError(timeoutError);

            throw new TimeoutException(timeoutError);
        }

        int exitCode = process.ExitCode;
        string standardOutput = outputBuilder.ToString();
        string standardError = errorBuilder.ToString();
        string logFinish = $"Finished with exit code {exitCode}: {fileName}";

        if (exitCode == 0)
        {
            __logger.LogInformation(logFinish);
        }
        else
        {
            __logger.LogError(logFinish);
        }

        var result = new ProcessResult(exitCode, standardOutput, standardError);

        return result;
    }
}