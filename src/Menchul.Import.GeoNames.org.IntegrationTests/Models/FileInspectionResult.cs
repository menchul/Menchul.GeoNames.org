namespace Menchul.Import.GeoNames.org.IntegrationTests.Models;

internal sealed class FileInspectionResult
{
    public string FileName { get; }

    public string FilePath { get; }

    public long FileSizeBytes { get; }

    public long LineCount { get; }

    public bool IsZipArchive { get; }

    public bool Exists { get; }

    public FileInspectionResult(string fileName, string filePath, long fileSizeBytes, long lineCount, bool isZipArchive, bool exists)
    {
        FileName = fileName;
        FilePath = filePath;
        FileSizeBytes = fileSizeBytes;
        LineCount = lineCount;
        IsZipArchive = isZipArchive;
        Exists = exists;
    }
}