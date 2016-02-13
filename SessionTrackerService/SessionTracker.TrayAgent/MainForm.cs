namespace SessionTracker.TrayAgent
{
    using System;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.notifyIcon.Click += NotifyIconOnClick;
        }

        private void NotifyIconOnClick(object sender, EventArgs eventArgs)
        {
            notifyIcon.ShowBalloonTip(1000, "Hello", "This is a balloontip!", ToolTipIcon.Info);
        }
    }
}
