using System;
using System.Drawing;
using System.Windows;

namespace TaskManager
{
    public partial class App : Application
    {
        System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        public App()
        {
            nIcon.Icon = new Icon("ico.ico", 32, 32);
            nIcon.Visible = true;
            nIcon.ShowBalloonTip(5000, "Планировщик задач", "Запуск произведен!", System.Windows.Forms.ToolTipIcon.Info);
            nIcon.Click += nIcon_Click;
        }

        void nIcon_Click(object sender, EventArgs e)
        {
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.WindowState = WindowState.Normal;
        }
    }
}