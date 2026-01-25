using Menchul.Import.GeoNames.org.Tools.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Menchul.Import.GeoNames.org.Tools
{
    internal class FileTools : IFileTools
    {
        private readonly ILogger<FileTools> __logger;
        private readonly ImporterParameters __importerParameters;

        public FileTools(ILogger<FileTools> logger, ImporterParameters importerParameters)
        {
            __importerParameters = importerParameters;
            __logger = logger;
        }

        public string CreateTempFolder()
        {
            string tempFolderName;

            if (string.IsNullOrWhiteSpace(__importerParameters.TempFolder))
            {
                //var tmp = Environment.GetEnvironmentVariable("TEMP");
                //var tmp = Environment.GetFolderPath(Environment.SpecialFolder.Templates);
                //var tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates);
                string tmp = Path.GetTempPath();

                tempFolderName = Path.Combine(tmp, "GeoNames.org");
                __importerParameters.TempFolder = tempFolderName;
            }
            else
            {
                tempFolderName = __importerParameters.TempFolder;
            }

            if (!__importerParameters.KeepTempFiles && Directory.Exists(tempFolderName))
            {
                Directory.Delete(tempFolderName, true);
            }

            if (!Directory.Exists(tempFolderName))
            {
                Directory.CreateDirectory(tempFolderName);
                bool isWindows = OperatingSystem.IsWindows();

                if (isWindows)
                {
                    GrantAccess(tempFolderName);
                }
            }

            __logger.LogDebug("Temp folder is: " + tempFolderName);

            return tempFolderName;
        }

#pragma warning disable CA1416
        private static void GrantAccess(string fullPath)
        {
            var directoryInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = directoryInfo.GetAccessControl();
            var identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            const InheritanceFlags inheritanceFlags = InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit;
            var rule = new FileSystemAccessRule(identity, FileSystemRights.FullControl, inheritanceFlags, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
            dSecurity.AddAccessRule(rule);
            directoryInfo.SetAccessControl(dSecurity);
        }
#pragma warning restore CA1416
    }
}