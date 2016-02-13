namespace SessionTracker.Service.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Dapper;

    using SessionTracker.Service.ConnectionProviders;
    using SessionTracker.Service.Entities;

    public class TrackerInstanceRepository : ITrackerInstanceRepository
    {
        private readonly IConnectionProvider connectionProvider;

        public TrackerInstanceRepository(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public Guid LogStartEvent(string machineName)
        {
            var trackerInstance = new TrackerInstance { Id = Guid.NewGuid(), StartAt = DateTime.UtcNow, LastUpdateAt = DateTime.UtcNow, MachineName = machineName };

            using (var connection = connectionProvider.GetConnection())
            {
                InsertTrackerInstanceInternal(connection, trackerInstance);
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

        public TrackerInstance Get(Guid trackerInstanceId)
        {
            const string SelectTrackerInstanceSqlQuery = @"
SELECT 
    Id,
    StartAt,
    LastUpdateAt,
    MachineName
FROM
    TrackerInstance
WHERE
    Id = @Id
";
            using (var connection = connectionProvider.GetConnection())
            {
                return connection.Query<TrackerInstance>(SelectTrackerInstanceSqlQuery, new { Id = trackerInstanceId }, commandType: CommandType.Text).Select(WithPatchedDates).SingleOrDefault();
            }
        }

        public IEnumerable<TrackerInstance> GetAll()
        {
            const string SelectAllTrackerInstanceSqlQuery = @"
SELECT 
    Id,
    StartAt,
    LastUpdateAt,
    MachineName
FROM
    TrackerInstance
";
            using (var connection = connectionProvider.GetConnection())
            {
                return connection.Query<TrackerInstance>(SelectAllTrackerInstanceSqlQuery, commandType: CommandType.Text).Select(WithPatchedDates);
            }
        }
        
        public void InsertTrackerInstance(TrackerInstance trackerInstance)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                InsertTrackerInstanceInternal(connection, trackerInstance);
            }
        }

        public void UpdateTrackerInstance(TrackerInstance trackerInstance)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                const string UpdateTrackerInstanceSqlQuery = @"
UPDATE TrackerInstance 
SET
    StartAt = @StartAt,
    LastUpdateAt = @LastUpdateAt,
    MachineName = @MachineName
WHERE 
    Id = @Id;
";
                connection.Execute(UpdateTrackerInstanceSqlQuery, trackerInstance, commandType: CommandType.Text);
            }
        }

        private static void InsertTrackerInstanceInternal(IDbConnection connection, TrackerInstance trackerInstance)
        {
            const string InsertTrackerInstanceSqlQuery = @"
INSERT INTO TrackerInstance(Id, StartAt, LastUpdateAt, MachineName) 
VALUES (@Id, @StartAt, @LastUpdateAt, @MachineName);
";

            connection.Execute(InsertTrackerInstanceSqlQuery, trackerInstance, commandType: CommandType.Text);
        }

        private static TrackerInstance WithPatchedDates(TrackerInstance trackerInstance)
        {
            trackerInstance.StartAt = DateTime.SpecifyKind(trackerInstance.StartAt, DateTimeKind.Utc);
            trackerInstance.LastUpdateAt = DateTime.SpecifyKind(trackerInstance.LastUpdateAt, DateTimeKind.Utc);
            return trackerInstance;
        }
    }
}
