using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Menchul.Import.GeoNames.org
{
    internal static class FileTools
    {
        public static string CreateTempFolder()
        {
            //var tmp = Environment.GetEnvironmentVariable("TEMP");
            //var tmp = Environment.GetFolderPath(Environment.SpecialFolder.Templates);
            //var tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates);
            string tmp = Path.GetTempPath();

            string tempFolderName = Path.Combine(tmp, "GeoNames.org");

            if (Directory.Exists(tempFolderName))
            {
                Directory.Delete(tempFolderName, true);
            }

            Directory.CreateDirectory(tempFolderName);

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                GrantAccess(tempFolderName);
            }

            Console.WriteLine("Temp folder is: " + tempFolderName);

            return tempFolderName;
        }

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
    }
}