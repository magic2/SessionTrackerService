namespace SessionTracker.Service.Services
{
    using System;
    using System.Data.SqlTypes;

    using NLog;

    using SessionTracker.Service.Repositories;

    public class DataSyncService : IDataSyncService
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly ITrackerInstanceRepository sourceTrackerInstanceRepository;
        private readonly ISessionLogRepository sourceSessionLogRepository;
        private readonly ITrackerInstanceRepository destinationTrackerInstanceRepository;
        private readonly ISessionLogRepository destinationSessionLogRepository;

        public DataSyncService(
            ITrackerInstanceRepository sourceTrackerInstanceRepository,
            ISessionLogRepository sourceSessionLogRepository,
            ITrackerInstanceRepository destinationTrackerInstanceRepository,
            ISessionLogRepository destinationSessionLogRepository)
        {
            this.sourceTrackerInstanceRepository = sourceTrackerInstanceRepository;
            this.sourceSessionLogRepository = sourceSessionLogRepository;
            this.destinationTrackerInstanceRepository = destinationTrackerInstanceRepository;
            this.destinationSessionLogRepository = destinationSessionLogRepository;
        }

        public void Sync()
        {
            try
            {
                this.SyncInternal();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Sync failed");
                return;
            }
        }

        private void SyncInternal()
        {
            foreach (var trackerInstance in this.sourceTrackerInstanceRepository.GetAll())
            {
                var destinationTrackerInstance = this.destinationTrackerInstanceRepository.Get(trackerInstance.Id);
                if (destinationTrackerInstance == null)
                {
                    this.destinationTrackerInstanceRepository.InsertTrackerInstance(trackerInstance);
                }
                else
                {
                    this.destinationTrackerInstanceRepository.UpdateTrackerInstance(trackerInstance);
                }

                var lastUpdateAt = destinationTrackerInstance?.LastUpdateAt ?? SqlDateTime.MinValue.Value;
                foreach (var sessionLog in
                    this.sourceSessionLogRepository.GetAllForInstanceLaterThan(trackerInstance.Id, lastUpdateAt))
                {
                    this.destinationSessionLogRepository.Save(sessionLog);
                }
            }
        }
    }
}
