namespace SessionTracker.Service
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            AfterInstall += OnAfterInstall;
        }

        private void OnAfterInstall(object sender, InstallEventArgs installEventArgs)
        {
            using (var sc = new ServiceController(serviceInstaller.ServiceName))
            {
                sc.Start();
            }
        }
    }
}
