namespace SessionTracker.Service.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.ServiceProcess;

    using Dapper;

    using SessionTracker.Service.ConnectionProviders;

    using SessionTracker.Service.Entities;

    public class SessionLogRepository : ISessionLogRepository
    {
        private readonly IConnectionProvider connectionProvider;

        public SessionLogRepository(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public void LogSessionEvent(Guid trackerInstanceId, int sessionId, UserInfo userInfo, SessionChangeReason sessionChangeReason)
        {
            Save(
                new SessionLogItem
                    {
                        CreatedAt = DateTime.UtcNow,
                        SessionChangeReason = sessionChangeReason.ToString(),
                        UserDomain = userInfo.Domain,
                        UserName = userInfo.Name,
                        SessionId = sessionId,
                        TrackerInstanceId = trackerInstanceId
                    });
        }

        public IEnumerable<SessionLogItem> GetAllForInstanceLaterThan(Guid trackerInstanceId, DateTime lastUpdateAt)
        {
            const string SelectSessionLogSqlQuery = @"
SELECT 
    TrackerInstanceId,
    SessionId,
    UserName,
    UserDomain,
    SessionChangeReason,
    CreatedAt
FROM
    SessionLog
WHERE
    TrackerInstanceId = @TrackerInstanceId
    AND
    CreatedAt >= @LastUpdateAt
";
            using (var connection = connectionProvider.GetConnection())
            {
                return connection.Query<SessionLogItem>(
                    SelectSessionLogSqlQuery,
                    new { TrackerInstanceId = trackerInstanceId, LastUpdateAt = lastUpdateAt },
                    commandType: CommandType.Text).Select(WithPatchedDates);
            }
        }

        public void Save(SessionLogItem sessionLogItem)
        {
            const string InsertSessionLogSqlQuery = @"
INSERT INTO SessionLog(TrackerInstanceId, SessionId, UserName, UserDomain, SessionChangeReason, CreatedAt) 
VALUES (@TrackerInstanceId, @SessionId, @UserName, @UserDomain, @SessionChangeReason, @CreatedAt);
";

            using (var connection = connectionProvider.GetConnection())
            {
                connection.Execute(InsertSessionLogSqlQuery, sessionLogItem, commandType: CommandType.Text);
            }
        }

        private static SessionLogItem WithPatchedDates(SessionLogItem sessionLogItem)
        {
            sessionLogItem.CreatedAt = DateTime.SpecifyKind(sessionLogItem.CreatedAt, DateTimeKind.Utc);
            return sessionLogItem;
        }
    }
}
