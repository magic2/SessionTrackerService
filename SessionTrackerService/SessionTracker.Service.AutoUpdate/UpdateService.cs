namespace SessionTracker.Service.AutoUpdate
{
    using System;
    using System.ServiceProcess;

    public partial class UpdateService : ServiceBase
    {
        public UpdateService()
        {
            InitializeComponent();
        }

        static void Main(string[] args)
        {
            var service = new UpdateService();

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
    }
}
