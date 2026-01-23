using Menchul.GeoNames.org;
using Menchul.GeoNames.org.MSSQL;
using Menchul.Import.GeoNames.org.Importers;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Menchul.Import.GeoNames.org
{
    public static class Program
    {
        private static GeoNamesOrgDbContext? __dbContext;

        [STAThread]
        private static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;
            Console.ResetColor();

            if (!args.Any())
            {
                CommandLineTools.ShowHelp();
                return;
            }

            ImporterParameters importerParameters = CommandLineTools.ParseCommandLineParameters(args);

            switch (importerParameters.Server)
            {
                case Server.MSSQL:
                    var msSQLDbContextFactory = new GeoNamesOrgMSSQLDbContextFactory(importerParameters.ConnectionString);
                    __dbContext = msSQLDbContextFactory.CreateDbContext();
                    break;
                case Server.PostgreSQL:
                    var postgreDbContextFactory = new GeoNamesOrgMSSQLDbContextFactory(importerParameters.ConnectionString);
                    __dbContext = postgreDbContextFactory.CreateDbContext();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    throw new NotImplementedException($"Please implement logic for the server \"{importerParameters.Server}\"");
            }

            if (importerParameters.TempFolder == null)
            {
                try
                {
                    importerParameters.TempFolder = FileTools.CreateTempFolder();
                }
                catch (Exception exception)
                {
                    CommandLineTools.WriteError(exception.Message);

                    return;
                }
            }

            Directory.CreateDirectory(importerParameters.TempFolder);

            ILoggerFactory factory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddNLog());

            Type[] importers =
            [
                typeof(ISOLanguagesImporter),
                typeof(FeatureCodesImporter),
                typeof(CountriesImporter),
                typeof(TimeZonesImporter),
                typeof(GeoNamesImporter),
                typeof(AlternateNamesV2Importer)
            ];

            foreach (Type importerType in importers)
            {
                ILogger logger = factory.CreateLogger(importerType);
                var objects = new object[] { __dbContext, logger, importerParameters };
                BaseImporter importer = (BaseImporter)Activator.CreateInstance(importerType, objects)!;

                await importer.DoImport();
            }

            if (importerParameters.NormalizeData)
            {
                await Normalizer.Normalize(__dbContext);
            }

            ReadKey();
        }

        [Conditional("DEBUG")]
        private static void ReadKey()
        {
            if (!Environment.UserInteractive || Console.IsInputRedirected || !Debugger.IsAttached)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"Application ended successfully");
            Console.WriteLine(@"*** Press any key to exit...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}