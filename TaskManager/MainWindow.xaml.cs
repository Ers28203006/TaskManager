using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TaskManager.DataAccess;

namespace TaskManager
{
    public partial class MainWindow : Window
    {
        List<Models.Task> tasks;
        public Models.Task Task { get; set; }
        public System.Threading.Timer Timer { get; set; }
        public DateTime Date { get; set; }
        public string PathAttachment { get; set; }
        public NotifyIcon NotifyIcon { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            tasks = new List<Models.Task>();

            Timer = new System.Threading.Timer(TimerCallback, null, 0, 60000);
        }

        private void TimerCallback(object o)
        {
           TaskPerfomance();
        }
        void TaskPerfomance()
        {
            using (var context = new TasksContext())
            {
                foreach (var task in context.Tasks)
                {
                    if (task.DateTime == DateTime.Now.ToString("g"))
                    {
                        if (task.IsSingly == true)
                        {
                            tasks.Add(task);
                            task.IsSingly = false;
                        }

                        if (task.IsDaily == true && task.DateTime == DateTime.Now.ToString("g"))
                        {
                            tasks.Add(task);
                            Date = DateTime.Parse(task.DateTime);
                            Date=Date.AddDays(1);
                            task.DateTime = Date.ToString("g");
                        }

                        if (task.IsWeekly == true && task.DateTime == DateTime.Now.ToString("g"))
                        {
                            tasks.Add(task);
                            Date = DateTime.Parse(task.DateTime);
                            Date = Date.AddMonths(1);
                            task.DateTime = Date.ToString("g");
                        }

                        if (task.IsAnnually == true && task.DateTime == DateTime.Now.ToString("g"))
                        {
                            tasks.Add(task);
                            Date = DateTime.Parse(task.DateTime);
                            Date = Date.AddYears(1);
                            task.DateTime = Date.ToString("g");
                        }
                    }

                    Date = DateTime.Parse(task.DateTime);
                    if (Date < DateTime.Now)
                    {
                        context.Tasks.Remove(task);
                    }
                }
                context.SaveChanges();
            }

            foreach (var task in tasks)
            {
                if (task.IsDownloadFile == true)
                {
                    Task = task;
                    Thread thread = new Thread(DownloadFiles);
                    thread.Start();
                }

                else if (task.IsMoveCatalog == true)
                {
                    Task = task;
                    Thread thread = new Thread(MoveFiles);
                    thread.Start();
                }

                else if (task.IsEmailSend == true)
                {
                    Task = task;
                    Thread thread = new Thread(EmailSend);
                    thread.Start();
                }
            }
        }

        #region все сервисы


        void DownloadFiles()
        {
            using (WebClient client = new WebClient())
            {
                string fileName = Path.GetFileName(Task.Url);
                WebClient webClient = new WebClient();
                string distination = Path.Combine(Task.WhereDownload, fileName);
                webClient.DownloadFileTaskAsync(Task.Url, distination);
            }
        }

        void MoveFiles()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Task.WhatCopyCatalog);

            DirectoryInfo destinationCatalog = new DirectoryInfo(Task.WhereCopyCatalog);

            if (dirInfo.Exists)
            {
                try
                {
                    if (!destinationCatalog.Exists)
                    {
                        destinationCatalog.Create();
                        dirInfo.MoveTo($@"{Task.WhereCopyCatalog}\ {dirInfo.Name}");
                    }
                    else
                    {
                        Directory.Delete(Task.WhereCopyCatalog, true);
                        destinationCatalog.Create();
                        dirInfo.MoveTo($@"{Task.WhereCopyCatalog}\ {dirInfo.Name}");
                    }
                }
                catch (IOException exception)
                {
                    System.Windows.MessageBox.Show(exception.Message);
                }
            }
        }

        void EmailSend()
        {
            try
            {
                MailAddress from = new MailAddress("ers28203006@gmail.com", "Ersyn");
                MailAddress to = new MailAddress(Task.ToEmail);
                using (MailMessage m = new MailMessage(from, to))
                using (SmtpClient smtp = new SmtpClient())
                {
                    try
                    {
                        m.Subject = Task.Thema;
                        m.Attachments.Add(new Attachment(PathAttachment));
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(from.Address, "Ceknfy,ftd33");
                        smtp.Send(m);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region события выбора Checked

        private void DownloadChecked(object sender, RoutedEventArgs e)
        {
            dowloadStackPanel.Visibility = Visibility.Visible;
            whatCopyCatalogTextBox.Text = "";
            whereCopyCatalogTextBox.Text = "";
            toEmailTextBox.Text = "";
            thema.Text = "";
            PathAttachment = "";
            moveStackPanel.Visibility = Visibility.Collapsed;
            emailStackPanel.Visibility = Visibility.Collapsed;
            moveRadioButton.IsChecked = false;
            emailRadioButton.IsChecked = false;
        }

        private void CatalogChecked(object sender, RoutedEventArgs e)
        {
            moveStackPanel.Visibility = Visibility.Visible;
            toEmailTextBox.Text = "";
            thema.Text = "";
            PathAttachment = "";
            urlTextBox.Text = "";
            whereDownloadTextBox.Text = "";
            emailStackPanel.Visibility = Visibility.Collapsed;
            dowloadStackPanel.Visibility = Visibility.Collapsed;
            downloadRadioButton.IsChecked = false;
            emailRadioButton.IsChecked = false;
        }

        private void EmailChecked(object sender, RoutedEventArgs e)
        {
            emailStackPanel.Visibility = Visibility.Visible;
            urlTextBox.Text = "";
            whereDownloadTextBox.Text = "";
            whatCopyCatalogTextBox.Text = "";
            whereCopyCatalogTextBox.Text = "";
            moveStackPanel.Visibility = Visibility.Collapsed;
            dowloadStackPanel.Visibility = Visibility.Collapsed;
            downloadRadioButton.IsChecked = false;
            moveRadioButton.IsChecked = false;
        }

        #endregion


        private void TaskButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Date = (DateTime)calendar.SelectedDate;
                DateTime time = (DateTime)timePicker.SelectedTime;
                Date = Date.AddHours(time.Hour).AddMinutes(time.Minute);
            }
            catch (Exception)
            {
                Date = DateTime.Now;
            }

            using (var context = new TasksContext())
            {
                context.Tasks.Add
                (
                    new Models.Task
                    {
                        DateTime = Date.ToString("g"),

                        IsDownloadFile = downloadRadioButton.IsChecked,
                        IsMoveCatalog = moveRadioButton.IsChecked,
                        IsEmailSend = emailRadioButton.IsChecked,

                        IsSingly = singlyRadioButton.IsChecked,
                        IsAnnually = annuallyRadioButton.IsChecked,
                        IsDaily = dailyRadioButton.IsChecked,
                        IsWeekly = weeklyRadioButton.IsChecked,

                        Url = urlTextBox.Text,
                        WhereDownload = whereDownloadTextBox.Text,

                        WhatCopyCatalog = whatCopyCatalogTextBox.Text,
                        WhereCopyCatalog = whereCopyCatalogTextBox.Text,

                        ToEmail = toEmailTextBox.Text,
                        Thema = thema.Text,
                        Attachment = PathAttachment
                    }
                );
                context.SaveChanges();
            }
        }

        private void AttachmentButtonClick(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "All Files (*.*)|*.*";

            if (openFile.ShowDialog() == true)
                PathAttachment = openFile.FileName;
        }

        private void WhatCopyCatalogTextBoxMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            whatCopyCatalogTextBox.Text = dialog.SelectedPath;
        }

        private void WhereCopyCatalogTextBoxMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            whereCopyCatalogTextBox.Text = dialog.SelectedPath;
        }

        private void WhereDownloadTextBoxMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            whereDownloadTextBox.Text = dialog.SelectedPath;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
