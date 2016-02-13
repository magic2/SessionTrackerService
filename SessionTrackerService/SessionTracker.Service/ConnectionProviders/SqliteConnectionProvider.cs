namespace SessionTracker.Service.ConnectionProviders
{
    using System.Data;
    using System.Data.SQLite;

    public class SqliteConnectionProvider : IConnectionProvider
    {
        public const string ProviderName = "System.Data.SQLite";

        private readonly string connectionString;

        public SqliteConnectionProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
