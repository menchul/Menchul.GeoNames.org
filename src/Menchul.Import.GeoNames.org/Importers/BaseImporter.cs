using Menchul.GeoNames.org;
using Microsoft.Extensions.Logging;
using ShellProgressBar;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.Importers
{
    internal abstract class BaseImporter
    {
        protected const string __baseUrl = @"http://download.geonames.org/export/dump/";

        protected static readonly Encoding __encoding = Encoding.UTF8;
        protected static readonly IFormatProvider __numberFormatInfo = new NumberFormatInfo();
        protected static readonly ProgressBarOptions __progressBarOptions = new()
        {
            BackgroundCharacter = '\u2592',// '\u2591',
            BackgroundColor = ConsoleColor.DarkGreen,
            ForegroundColor = ConsoleColor.Green,
            EnableTaskBarProgress = true,
            CollapseWhenFinished = true,
            PercentageFormat = "{0:N0}% "
        };


        protected readonly GeoNamesOrgDbContext __dbContext;
        protected readonly ILogger __logger;
        protected readonly ImporterParameters __importerParameters;

        protected ProgressBar _pbar;

        protected BaseImporter(
            GeoNamesOrgDbContext dbContext,
            ILogger logger,
            ImporterParameters importerParameters)
        {
            __dbContext = dbContext;
            __logger = logger;
            __importerParameters = importerParameters;
        }

        protected abstract string FileURL { get; }
        protected virtual ulong FirstRow { get; } = 1;

        protected string DownloadFileName => Path.GetFileName(FileURL);

        protected string DownloadFileExtension => Path.GetExtension(FileURL);

        protected string DownloadFileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileURL);

        protected string LocalFileName => Path.Combine(__importerParameters.TempFolder, DownloadFileName);

        protected string ArchiveFolder => Path.Combine(__importerParameters.TempFolder, DownloadFileNameWithoutExtension);

        public bool IsZIPed => string.Equals(".zip", DownloadFileExtension, StringComparison.InvariantCultureIgnoreCase);

        public async Task DoImport()
        {
            var sw = new Stopwatch();
            sw.Start();

            __logger.LogInformation($"STARTed at {DateTime.Now}");

            try
            {
                await DownloadFile(FileURL, LocalFileName);

                if (!Directory.Exists(ArchiveFolder))
                {
                    if (IsZIPed)
                    {
                        __logger.LogTrace($"File \"{LocalFileName}\" started UnZIPing...");

                        ZipFile.ExtractToDirectory(LocalFileName, ArchiveFolder);

                        __logger.LogTrace($"File \"{LocalFileName}\" UnZIPed.");
                    }
                }

                await ImportData();
            }
            catch (Exception exception)
            {
                __logger.LogError(exception, exception.Message);
            }

            sw.Stop();

            __logger.LogInformation($"ENDed at {DateTime.Now} and took {sw.Elapsed:mm\\m\\ ss\\s}");
        }

        protected abstract Task ImportData();


        protected static string GetNullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        protected async Task DownloadFile(string url, string fileName)
        {
            __logger.LogTrace($"DownloadFile init \"{fileName}\" from \"{url}\"");

            if (File.Exists(fileName))
            {
                __logger.LogTrace($"File \"{fileName}\" already exists for the URL \"{url}\"");

                return;
            }

            __logger.LogTrace($"Started file \"{fileName}\" download from URL \"{url}\"");

            string file = Path.GetFileName(fileName);

            using (_pbar = new(100, "", __progressBarOptions))
            {
                using (var client = new ExtendedWebClient())
                {
                    client.FileName = file;
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;

                    await client.DownloadFileTaskAsync(url, fileName);

                    client.DownloadProgressChanged -= Client_DownloadProgressChanged;
                }
            }

            __logger.LogTrace($"File \"{fileName}\" just downloaded.");
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var ewc = (ExtendedWebClient)sender;

            _pbar.Tick(e.ProgressPercentage, $"{ewc.FileName} - bytes received {e.BytesReceived:### ### ###} from total {e.TotalBytesToReceive:### ### ###} bytes.");
        }
    }
}