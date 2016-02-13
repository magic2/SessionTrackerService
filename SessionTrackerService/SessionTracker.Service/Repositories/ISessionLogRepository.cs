namespace SessionTracker.Service.Repositories
{
    using System;
    using System.ServiceProcess;

    public interface ISessionLogRepository
    {
        void LogSessionEvent(Guid trackerInstanceId, int sessionId, string userName, string userDomain, SessionChangeReason sessionChangeReason);
    }
}
