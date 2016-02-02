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

        public void LogSessionEvent(int sessionId, string userName, string userDomain, SessionChangeReason sessionChangeReason)
        {
            const string InsertSessionLogSqlQuery = @"
INSERT SessionLog(SessionId, UserName, UserDomain, SessionChangeReason, CreatedAt) 
VALUES (@SessionId, @UserName, @UserDomain, @SessionChangeReason, @CreatedAt);
";
            var sessionLogItem = new SessionLogItem
                                     {
                                         CreatedAt = DateTime.UtcNow,
                                         SessionChangeReason = sessionChangeReason.ToString(),
                                         UserDomain = userDomain,
                                         UserName = userName,
                                         SessionId = sessionId
                                     };

            using (var connection = connectionProvider.GetConnection())
            {
                connection.Execute(InsertSessionLogSqlQuery, sessionLogItem, commandType: CommandType.Text);
            }}
    }
}
