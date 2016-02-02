namespace SessionTracker.Service.ConnectionProviders
{
    using System.Data;

    public interface IConnectionProvider
    {
        IDbConnection GetConnection();
    }
}
