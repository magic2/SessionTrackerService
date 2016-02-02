namespace SessionTracker.Service.ConnectionProviders
{
    using System.Data;

    using MySql.Data.MySqlClient;

    public class MySqlConnectionProvider : IConnectionProvider
    {
        public const string ProviderName = "MySql.Data.MySqlClient";

        private readonly string connectionString;

        public MySqlConnectionProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }
        
        public IDbConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
