namespace SessionTracker.Service
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public class FileSystemStructureProvider : IFileSystemStructureProvider
    {
        public string GetDataDirectoryPath()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), versionInfo.CompanyName, versionInfo.ProductName);
        }
    }
}
