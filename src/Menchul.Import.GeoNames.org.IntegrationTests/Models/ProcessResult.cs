namespace Menchul.Import.GeoNames.org.IntegrationTests.Models;

internal sealed class ProcessResult
{
    public int ExitCode { get; }

    public string StandardOutput { get; }

    public string StandardError { get; }

    public ProcessResult(int exitCode, string standardOutput, string standardError)
    {
        ExitCode = exitCode;
        StandardOutput = standardOutput;
        StandardError = standardError;
    }
}