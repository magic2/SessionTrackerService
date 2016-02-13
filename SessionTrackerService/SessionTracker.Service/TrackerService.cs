namespace SessionTracker.Service
{
    using System;
    using System.ServiceProcess;
    using System.Timers;

    using NLog;

    using SessionTracker.Service.ConnectionProviders;
    using SessionTracker.Service.Repositories;
    using SessionTracker.Service.Services;

    public partial class TrackerService : ServiceBase
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IUserInfoService userInfoService;
        private readonly ISessionLogRepository sessionLogRepository;
        private readonly ITrackerInstanceRepository trackerInstanceRepository;
        private readonly ISessionLogRepository localSessionLogRepository;
        private readonly ITrackerInstanceRepository localTrackerInstanceRepository;
        private readonly IDataSyncService dataSyncService;
        private readonly IDatabaseCreatorService databaseCreatorService;
        private readonly IFileSystemStructureProvider fileSystemStructureProvider;

        private Guid trackerInstanceId;

        private Timer timer;

        private TrackerService()
        {
            InitializeComponent();

            userInfoService = new UserInfoService();
            fileSystemStructureProvider = new FileSystemStructureProvider();
            
            var remoteConnectionProvider = new ConnectionProviderFactory("Default").GetConnectionProvider();
            sessionLogRepository = new SessionLogRepository(remoteConnectionProvider);
            trackerInstanceRepository = new TrackerInstanceRepository(remoteConnectionProvider);

            var localConnectionProvider = new ConnectionProviderFactory("Local").GetConnectionProvider();
            databaseCreatorService = new DatabaseCreatorService(localConnectionProvider, fileSystemStructureProvider);

            localSessionLogRepository = new SessionLogRepository(localConnectionProvider);
            localTrackerInstanceRepository = new TrackerInstanceRepository(localConnectionProvider);

            dataSyncService = new DataSyncService(
                localTrackerInstanceRepository,
                localSessionLogRepository,
                trackerInstanceRepository,
                sessionLogRepository
                );
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", new FileSystemStructureProvider().GetDataDirectoryPath());
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            var service = new TrackerService();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);
                Console.WriteLine("Press any key to stop program");
                Console.Read();
                service.OnStop();
            }
            else
            {
                Run(service);
            }
        }

        protected override void OnStart(string[] args)
        {
            // System.Diagnostics.Debugger.Launch();

            databaseCreatorService.EnsureSchema();
            trackerInstanceId = localTrackerInstanceRepository.LogStartEvent(System.Environment.MachineName);

            timer = new Timer { Interval = TimeSpan.FromMinutes(1).TotalMilliseconds };
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
            var userInfo = userInfoService.GetUserInformationbySessionId(changeDescription.SessionId);

            if (userInfo == null)
            {
                Log.Error("Session Change: UserInfo is not provided");
                return;
            }

            localSessionLogRepository.LogSessionEvent(trackerInstanceId, changeDescription.SessionId, userInfo, changeDescription.Reason);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            localTrackerInstanceRepository.UpdateState(trackerInstanceId);

            dataSyncService.Sync();
        }
    }
}
