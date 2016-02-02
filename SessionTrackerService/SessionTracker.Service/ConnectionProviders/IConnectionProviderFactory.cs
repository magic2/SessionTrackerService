namespace SessionTracker.Service.ConnectionProviders
{
    public interface IConnectionProviderFactory
    {
        IConnectionProvider GetConnectionProvider();
    }
}
