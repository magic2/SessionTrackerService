namespace SessionTracker.Service.Repositories
{
    using System;

    public interface ITrackerInstanceRepository
    {
        Guid LogStartEvent(string machineName);

        void UpdateState(Guid trackerInstanceId);
    }
}