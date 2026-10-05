using Menchul.Import.GeoNames.org.IntegrationTests.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal sealed class FileDownloadManager
{
    private static readonly string[] __downloadUrls =
    [
        DumpFileNames.BaseUrl + DumpFileNames.IsoLanguageCodes,
        DumpFileNames.BaseUrl + DumpFileNames.FeatureCodesEn,
        DumpFileNames.BaseUrl + DumpFileNames.CountryInfo,
        DumpFileNames.BaseUrl + DumpFileNames.TimeZones,
        DumpFileNames.BaseUrl + DumpFileNames.AllCountriesZip,
        DumpFileNames.BaseUrl + DumpFileNames.AlternateNamesV2Zip
    ];

    private readonly ILogger __logger;

    public FileDownloadManager(ILogger logger)
    {
        __logger = logger;
    }

    public async Task<IReadOnlyList<FileInspectionResult>> DownloadAndInspectFilesAsync(string tempFolder)
    {
        bool dirExists = Directory.Exists(tempFolder);

        if (!dirExists)
        {
            Directory.CreateDirectory(tempFolder);
        }

        var results = new List<FileInspectionResult>();
        using var httpClient = new HttpClient();

        foreach (string url in __downloadUrls)
        {
            string fileName = Path.GetFileName(url);
            string destinationPath = Path.Combine(tempFolder, fileName);
            bool fileExists = File.Exists(destinationPath);

            if (!fileExists)
            {
                string downloadLog = $"Downloading file \"{fileName}\" from \"{url}\"...";
                __logger.LogInformation(downloadLog);

                using HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using Stream remoteStream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await remoteStream.CopyToAsync(fileStream);

                string doneLog = $"Downloaded \"{fileName}\" successfully.";
                __logger.LogInformation(doneLog);
            }
            else
            {
                string cachedLog = $"File \"{fileName}\" already exists in temp folder.";
                __logger.LogInformation(cachedLog);
            }

            var fileInfo = new FileInfo(destinationPath);
            long fileSizeBytes = fileInfo.Length;
            bool isZip = fileName.EndsWith(IntegrationTestConstants.Paths.ZipExtension, StringComparison.OrdinalIgnoreCase);
            long lineCount = await CountLinesAsync(destinationPath, isZip);

            double sizeMb = (double)fileSizeBytes / (1024 * 1024);
            string inspectionLog = $"File: {fileName} | Size: {sizeMb:F2} MB | Lines: {lineCount:N0}";
            __logger.LogInformation(inspectionLog);

            var inspectionResult = new FileInspectionResult(fileName, destinationPath, fileSizeBytes, lineCount, isZip, true);
            results.Add(inspectionResult);
        }

        return results;
    }

    private static async Task<long> CountLinesAsync(string filePath, bool isZip)
    {
        long count = 0;

        if (isZip)
        {
            await using FileStream zipStream = File.OpenRead(filePath);
            await using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                bool isTextEntry = entry.Name.EndsWith(IntegrationTestConstants.Paths.TextExtension, StringComparison.OrdinalIgnoreCase);

                if (!isTextEntry)
                {
                    continue;
                }

                await using Stream entryStream = await entry.OpenAsync();

                using var reader = new StreamReader(entryStream);

                while (await reader.ReadLineAsync() != null)
                {
                    count++;
                }

                break;
            }
        }
        else
        {
            await using FileStream fileStream = File.OpenRead(filePath);

            using var reader = new StreamReader(fileStream);

            while (await reader.ReadLineAsync() != null)
            {
                count++;
            }
        }

        return count;
    }
}