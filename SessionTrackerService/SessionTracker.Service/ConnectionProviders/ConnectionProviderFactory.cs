namespace SessionTracker.Service.ConnectionProviders
{
    using System.Configuration;

    public class ConnectionProviderFactory : IConnectionProviderFactory
    {
        private const string ConnectionStringName = "Default";

        public IConnectionProvider GetConnectionProvider()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            switch (connectionString.ProviderName)
            {
                case MySqlConnectionProvider.ProviderName:
                    return new MySqlConnectionProvider(connectionString.ConnectionString);
                case SqlServerConnectionProvider.ProviderName:
                    return new SqlServerConnectionProvider(connectionString.ConnectionString);
                default:
                    throw new ConfigurationErrorsException($"The {connectionString.ProviderName} provider is not supported.");
            }
        }
    }
}
