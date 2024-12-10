using Import.GeoNames.org.Properties;
using System;

namespace Menchul.Import.GeoNames.org
{
    internal static class CommandLineTools
    {
        public static ImporterParameters ParseCommandLineParameters(string[] args)
        {
            var importParameters = new ImporterParameters();

            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i];
                argument = argument.Trim();

                switch (argument)
                {
                    case "/?":
                    case "-h":
                    case "/h":
                    case "--help":
                    case "/help":
                        ShowHelp();

                        return null;
                    case "--server":
                        i++;

                        if (args.Length < i + 1)
                        {
                            WriteError("Bad Server");

                            return null;
                        }

                        string srv = args[i].Trim();

                        if (string.Equals("MSSQL", srv, StringComparison.InvariantCultureIgnoreCase))
                        {
                            importParameters.Server = Server.MSSQL;

                            continue;
                        }

                        if (string.Equals("PostgreSQL", srv, StringComparison.InvariantCultureIgnoreCase))
                        {
                            importParameters.Server = Server.PostgreSQL;
                            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                            continue;
                        }

                        break;
                    case "--connectionString":
                    case "-cs":
                        i++;

                        if (args.Length < i + 1)
                        {
                            WriteError(Resources.BAD_CONNECTION_STRING);

                            return null;
                        }

                        string connectionString = args[i];

                        if (string.IsNullOrWhiteSpace(connectionString))
                        {
                            WriteError(Resources.BAD_CONNECTION_STRING);

                            return null;
                        }

                        importParameters.ConnectionString = connectionString;

                        break;
                    case "--tempFolder":
                    case "-tf":
                        i++;

                        if (args.Length < i + 1)
                        {
                            WriteError(Resources.BAD_TEMPORARY_FOLDER);

                            return null;
                        }

                        string tempFolderName = args[i];
                        bool correctTempFolder = !string.IsNullOrWhiteSpace(tempFolderName);

                        if (!correctTempFolder)
                        {
                            WriteError(Resources.BAD_TEMPORARY_FOLDER);

                            return null;
                        }

                        importParameters.TempFolder = tempFolderName;

                        break;
                    case "--importOnlyAP":
                        importParameters.ImportOnlyAP = true;
                        break;
                    case "--ignoreBadNames":
                        importParameters.IgnoreBadNames = true;
                        break;
                }
            }

            return importParameters;
        }

        public static void ShowHelp()
        {
            Console.WriteLine(@"-c , -connectionString      Connection String for connecting to MS SQL Server.");
            Console.WriteLine(@"-tf, -tempFolder            Temporary folder, where files will be saved. If not set. will be used system TEMP folder.");
            Console.WriteLine(@"-cleardb                    Clear whall DataBase.");
        }

        public static void WriteError(string errorMessage)
        {
            lock (Console.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(errorMessage);
                Console.ResetColor();
            }

            Console.ReadKey();
        }
    }
}