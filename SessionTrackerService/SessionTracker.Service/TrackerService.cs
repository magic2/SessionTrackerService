namespace SessionTracker.Service
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;
    using System.Threading.Tasks;

    using NLog;

    using SessionTracker.Service.ConnectionProviders;
    using SessionTracker.Service.Repositories;

    public partial class TrackerService : ServiceBase
    {
        public enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo,
            WTSConfigInfo,
            WTSValidationInfo,
            WTSSessionAddressV4,
            WTSIsRemoteSession
        }

        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(System.IntPtr pServer, int iSessionID, WTS_INFO_CLASS oInfoClass, out System.IntPtr pBuffer, out uint iBytesReturned);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly SessionLogRepository sessionLogRepository;

        private class User
        {
            public string Name { get; set; }

            public string Domain { get; set; }

            public string ToString()
            {
                return $"{Name}\\{Domain}";
            }
        }

        public TrackerService()
        {
            InitializeComponent();

            sessionLogRepository = new SessionLogRepository(new ConnectionProviderFactory());
        }

        static void Main(string[] args)
        {
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
        }

        protected override void OnStop()
        {
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
            var user = UserInformation(changeDescription.SessionId);

            sessionLogRepository.LogSessionEvent(changeDescription.SessionId, user.Name, user.Domain, changeDescription.Reason);
        }

        private static User UserInformation(int sessionId)
        {
            IntPtr buffer;
            uint length;

            var user = new User();

            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out length) && length > 1)
            {
                user.Name = Marshal.PtrToStringAnsi(buffer);

                WTSFreeMemory(buffer);
                if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSDomainName, out buffer, out length) && length > 1)
                {
                    user.Domain = Marshal.PtrToStringAnsi(buffer);
                    WTSFreeMemory(buffer);
                }
            }

            if (user.Name.Length == 0)
            {
                return null;
            }

            return user;
        }
    }
}
