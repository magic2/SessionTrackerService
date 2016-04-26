namespace SessionTracker.TrayAgent
{
    using System;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private bool allowVisible;
        private bool allowClose;

        public MainForm()
        {
            InitializeComponent();
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            toolExit.Click += ToolExitClick;
            toolShow.Click += ToolShowClick;
            toolHide.Click += ToolHideClick;
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!IsHandleCreated) CreateHandle();
            }

            base.SetVisibleCore(value);
        }

        private void ToolHideClick(object sender, EventArgs e)
        {
            Hide();
            toolHide.Visible = false;
            toolShow.Visible = true;
        }

        private void ToolShowClick(object sender, EventArgs e)
        {
            allowVisible = true;
            Show();
            toolHide.Visible = true;
            toolShow.Visible = false;
        }

        private void ToolExitClick(object sender, EventArgs e)
        {
            allowClose = true;
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                Hide();
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }

    }
}
