namespace SessionTracker.Service.ConnectionProviders
{
    using System.Configuration;

    public class ConnectionProviderFactory : IConnectionProviderFactory
    {
        private readonly string connectionStringName;

        public ConnectionProviderFactory() : this("Default")
        {
        }

        public ConnectionProviderFactory(string connectionStringName)
        {
            this.connectionStringName = connectionStringName;
        }

        public IConnectionProvider GetConnectionProvider()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            switch (connectionString.ProviderName)
            {
                case MySqlConnectionProvider.ProviderName:
                    return new MySqlConnectionProvider(connectionString.ConnectionString);
                case SqlServerConnectionProvider.ProviderName:
                    return new SqlServerConnectionProvider(connectionString.ConnectionString);
                case SqliteConnectionProvider.ProviderName:
                    return new SqliteConnectionProvider(connectionString.ConnectionString);
                default:
                    throw new ConfigurationErrorsException($"The {connectionString.ProviderName} provider is not supported.");
            }
        }
    }
}
