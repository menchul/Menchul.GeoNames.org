using Import.GeoNames.org.Properties;
using System;

namespace Menchul.Import.GeoNames.org;

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
                case CommandLineConstants.HelpQuestion:
                case CommandLineConstants.HelpShort:
                case CommandLineConstants.HelpSlash:
                case CommandLineConstants.HelpLong:
                case CommandLineConstants.HelpWord:
                    ShowHelp();

                    throw new Exception();
                case CommandLineConstants.Server:
                    i++;

                    if (args.Length < i + 1)
                    {
                        WriteError("Bad Server");

                        throw new Exception();
                    }

                    string srv = args[i].Trim();

                    if (!Enum.TryParse(srv, true, out Server serverType))
                    {
                        string message = $"DB server \"{srv}\" is not recognized";

                        throw new ArgumentOutOfRangeException(CommandLineConstants.Server, message);
                    }

                    importParameters.Server = serverType;

                    switch (serverType)
                    {
                        case Server.PostgreSQL:
                            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                            break;
                    }

                    break;
                case CommandLineConstants.ConnectionString:
                case CommandLineConstants.Connection:
                case CommandLineConstants.ConnectionStringShort:
                    i++;

                    if (args.Length < i + 1)
                    {
                        WriteError(Resources.BAD_CONNECTION_STRING);

                        throw new Exception();
                    }

                    string connectionString = args[i];

                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        WriteError(Resources.BAD_CONNECTION_STRING);

                        throw new Exception();
                    }

                    importParameters.ConnectionString = connectionString;

                    break;
                case CommandLineConstants.TempFolder:
                case CommandLineConstants.TempFolderShort:
                    i++;

                    if (args.Length < i + 1)
                    {
                        WriteError(Resources.BAD_TEMPORARY_FOLDER);

                        throw new Exception();
                    }

                    string tempFolderName = args[i];
                    bool correctTempFolder = !string.IsNullOrWhiteSpace(tempFolderName);

                    if (!correctTempFolder)
                    {
                        WriteError(Resources.BAD_TEMPORARY_FOLDER);

                        throw new Exception();
                    }

                    importParameters.TempFolder = tempFolderName;

                    break;
                case CommandLineConstants.ImportOnlyAP:
                    importParameters.ImportOnlyAP = true;
                    break;
                case CommandLineConstants.NormalizeData:
                    importParameters.NormalizeData = true;
                    break;
                case CommandLineConstants.KeepTempFiles:
                    importParameters.KeepTempFiles = true;
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