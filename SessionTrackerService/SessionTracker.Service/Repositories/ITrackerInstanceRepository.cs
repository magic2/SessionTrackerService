namespace SessionTracker.Service.Repositories
{
    using System;
    using System.Collections.Generic;

    using SessionTracker.Service.Entities;

    public interface ITrackerInstanceRepository
    {
        Guid LogStartEvent(string machineName);

        void UpdateState(Guid trackerInstanceId);

        TrackerInstance Get(Guid trackerInstanceId);

        IEnumerable<TrackerInstance> GetAll();

        void InsertTrackerInstance(TrackerInstance trackerInstance);

        void UpdateTrackerInstance(TrackerInstance trackerInstance);
    }
}