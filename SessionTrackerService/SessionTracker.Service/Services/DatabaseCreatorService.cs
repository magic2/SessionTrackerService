namespace SessionTracker.Service.Services
{
    using System.Data.SQLite;
    using System.IO;

    using Dapper;

    using NLog;

    using SessionTracker.Service.ConnectionProviders;

    public class DatabaseCreatorService : IDatabaseCreatorService
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IConnectionProvider connectionProvider;
        private readonly IFileSystemStructureProvider fileSystemStructureProvider;

        public DatabaseCreatorService(IConnectionProvider connectionProvider, IFileSystemStructureProvider fileSystemStructureProvider)
        {
            this.connectionProvider = connectionProvider;
            this.fileSystemStructureProvider = fileSystemStructureProvider;
        }

        public void EnsureSchema()
        {
            var localDatabaseFileName = Path.Combine(this.fileSystemStructureProvider.GetDataDirectoryPath(), "SessionTracker.db");
            if (File.Exists(localDatabaseFileName))
            {
                Log.Info($"Local database exists {localDatabaseFileName}");
                return;
            }

            Log.Info($"Creating local database {localDatabaseFileName}");
            SQLiteConnection.CreateFile(localDatabaseFileName);
            using (var connection = this.connectionProvider.GetConnection())
            {
                connection.Execute(SessionTracker.Service.Properties.Resources.SessionTracker);
                connection.Close();
            }

            Log.Info($"Local database created");
        }
    }
}
