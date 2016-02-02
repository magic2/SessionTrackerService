namespace SessionTracker.Service.Repositories
{
    using System.ServiceProcess;

    public interface ISessionLogRepository
    {
        void LogSessionEvent(int sessionId, string userName, string userDomain, SessionChangeReason sessionChangeReason);
    }
}
