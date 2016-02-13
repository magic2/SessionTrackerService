namespace SessionTracker.Service.Entities
{
    using System;

    public class TrackerInstance
    {
        public Guid Id { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string MachineName { get; set; }
    }
}
