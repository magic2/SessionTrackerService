namespace SessionTracker.Service.ConnectionProviders
{
    using System.Data;
    using System.Data.SqlClient;

    public class SqlServerConnectionProvider : IConnectionProvider
    {
        public const string ProviderName = "System.Data.SqlClient";

        private readonly string connectionString;

        public SqlServerConnectionProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
