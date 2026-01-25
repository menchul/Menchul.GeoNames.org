using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Menchul.Import.GeoNames.org
{
    internal static class FileTools
    {
        public static string CreateTempFolder(ImporterParameters importerParameters)
        {
            string tempFolderName;

            if (string.IsNullOrWhiteSpace(importerParameters.TempFolder))
            {
                //var tmp = Environment.GetEnvironmentVariable("TEMP");
                //var tmp = Environment.GetFolderPath(Environment.SpecialFolder.Templates);
                //var tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates);
                string tmp = Path.GetTempPath();

                tempFolderName = Path.Combine(tmp, "GeoNames.org");
            }
            else
            {
                tempFolderName = importerParameters.TempFolder;
            }

            if (!importerParameters.KeepTempFiles && Directory.Exists(tempFolderName))
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

            Console.WriteLine("Temp folder is: " + tempFolderName);

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