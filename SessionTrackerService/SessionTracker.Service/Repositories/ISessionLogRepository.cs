namespace SessionTracker.Service.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;

    using SessionTracker.Service.Entities;

    public interface ISessionLogRepository
    {
        void LogSessionEvent(Guid trackerInstanceId, int sessionId, UserInfo userInfo, SessionChangeReason sessionChangeReason);

        IEnumerable<SessionLogItem> GetAllForInstanceLaterThan(Guid trackerInstanceId, DateTime lastUpdateAt);

        void Save(SessionLogItem sessionLogItem);
    }
}
