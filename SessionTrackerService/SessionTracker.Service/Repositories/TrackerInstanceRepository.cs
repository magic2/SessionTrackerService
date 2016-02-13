namespace SessionTracker.Service.Repositories
{
    using System;
    using System.Data;

    using Dapper;

    using SessionTracker.Service.ConnectionProviders;
    using SessionTracker.Service.Entities;

    public class TrackerInstanceRepository : ITrackerInstanceRepository
    {
        private readonly IConnectionProvider connectionProvider;

        public TrackerInstanceRepository(IConnectionProviderFactory connectionProviderFactory)
        {
            connectionProvider = connectionProviderFactory.GetConnectionProvider();
        }

        public Guid LogStartEvent(string machineName)
        {
            const string InsertTrackerInstanceSqlQuery = @"
INSERT TrackerInstance(Id, StartAt, LastUpdateAt, MachineName) 
VALUES (@Id, @StartAt, @LastUpdateAt, @MachineName);
";
            var trackerInstance = new TrackerInstance { Id = Guid.NewGuid(), StartAt = DateTime.UtcNow, LastUpdateAt = DateTime.UtcNow, MachineName = machineName };

            using (var connection = connectionProvider.GetConnection())
            {
                connection.Execute(InsertTrackerInstanceSqlQuery, trackerInstance, commandType: CommandType.Text);
            }

            return trackerInstance.Id;
        }

        public void UpdateState(Guid trackerInstanceId)
        {
            const string UpdateTrackerInstanceSqlQuery = @"UPDATE TrackerInstance SET LastUpdateAt = @LastUpdateAt WHERE Id = @Id";
            using (var connection = connectionProvider.GetConnection())
            {
                connection.Execute(UpdateTrackerInstanceSqlQuery, new { Id = trackerInstanceId, LastUpdateAt = DateTime.UtcNow }, commandType: CommandType.Text);
            }
        }
    }
}
