namespace SessionTracker.Service.Repositories
{
    using System;
    using System.Data;
    using System.ServiceProcess;

    using Dapper;

    using SessionTracker.Service.ConnectionProviders;

    using SessionTracker.Service.Entities;

    public class SessionLogRepository : ISessionLogRepository
    {
        private readonly IConnectionProvider connectionProvider;

        public SessionLogRepository(IConnectionProviderFactory connectionProviderFactory)
        {
            connectionProvider = connectionProviderFactory.GetConnectionProvider();
        }

        public void LogSessionEvent(Guid trackerInstanceId, int sessionId, string userName, string userDomain, SessionChangeReason sessionChangeReason)
        {
            const string InsertSessionLogSqlQuery = @"
INSERT SessionLog(TrackerInstanceId, SessionId, UserName, UserDomain, SessionChangeReason, CreatedAt) 
VALUES (@TrackerInstanceId, @SessionId, @UserName, @UserDomain, @SessionChangeReason, @CreatedAt);
";
            var sessionLogItem = new SessionLogItem
                                     {
                                         CreatedAt = DateTime.UtcNow,
                                         SessionChangeReason = sessionChangeReason.ToString(),
                                         UserDomain = userDomain,
                                         UserName = userName,
                                         SessionId = sessionId,
                                         TrackerInstanceId = trackerInstanceId
                                     };

            using (var connection = connectionProvider.GetConnection())
            {
                connection.Execute(InsertSessionLogSqlQuery, sessionLogItem, commandType: CommandType.Text);
            }
        }
    }
}
