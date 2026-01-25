using Menchul.GeoNames.org;
using Menchul.GeoNames.org.MSSQL;
using Menchul.GeoNames.org.PostgreSQL;
using Menchul.Import.GeoNames.org.Importers;
using Menchul.Import.GeoNames.org.Importers.Base;
using Menchul.Import.GeoNames.org.Tools;
using Menchul.Import.GeoNames.org.Tools.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    var postgreDbContextFactory = new GeoNamesOrgPostgreSQLDbContextFactory(importerParameters.ConnectionString);
                    __dbContext = postgreDbContextFactory.CreateDbContext();
                    break;
                default:
                    string message = $"Please implement logic for the server \"{importerParameters.Server}\"";

                    CommandLineTools.WriteError(message);

                    throw new NotImplementedException(message);
            }

            await __dbContext.Database.EnsureCreatedAsync();

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(importerParameters);
            services.AddLogging(builder => builder.AddNLog());
            services.AddScoped<GeoNamesOrgDbContext>(_ => __dbContext);
            services.AddTransient<IFileTools, FileTools>();
            services.AddTransient<INormalizer, Normalizer>();
            services.AddScoped<BaseImporter, ISOLanguagesImporter>();
            services.AddScoped<BaseImporter, FeatureCodesImporter>();
            services.AddScoped<BaseImporter, CountriesImporter>();
            services.AddScoped<BaseImporter, TimeZonesImporter>();
            services.AddScoped<BaseImporter, GeoNamesImporter>();
            services.AddScoped<BaseImporter, AlternateNamesV2Importer>();

            await using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFileTools fileTools = serviceProvider.GetService<IFileTools>()!;
            fileTools.CreateTempFolder();

            using IServiceScope scope = serviceProvider.CreateScope();
            BaseImporter[] importers = scope.ServiceProvider.GetServices<BaseImporter>().OrderBy(i => i.Order).ToArray();

            foreach (BaseImporter importer in importers)
            {
                await importer.DoImport();
            }

            if (importerParameters.NormalizeData)
            {
                INormalizer normalizer = serviceProvider.GetService<INormalizer>()!;

                await normalizer.Normalize();
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