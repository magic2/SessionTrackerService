namespace SessionTracker.Service.Entities
{
    using System;

    public class SessionLogItem
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string UserName { get; set; }
        public string UserDomain { get; set; }
        public string SessionChangeReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid TrackerInstanceId { get; set; }
    }
}
