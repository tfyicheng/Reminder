using NtpTestDemo;
using System;
using System.ComponentModel;
using System.Windows;

namespace Reminder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //public static Set set = new Set();

        private ReminderModel reminder { set; get; } = new ReminderModel();

        myIcon ic;
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = reminder;

            reminder.test();

            ic = new myIcon();
            ic.Icon();

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            ////ic.CloseWindow(null, null);
            //System.Diagnostics.Process.GetCurrentProcess().Kill();


            //  Environment.Exit(0);
            //  Application.Current.Shutdown();
        }

        private void DeletDate(object sender, RoutedEventArgs e)
        {
            try
            {
                //String.IsNullOrEmpty(timePicker.Text)
                if (true)
                {
                    Console.WriteLine(timePicker.SelectedDateTime);
                    HandyControl.Controls.Growl.Info(timePicker.Text);
                    //Growl.Info("test");
                    //Notification.Show(new AppNotification(), ShowAnimation.HorizontalMove, true);
                    //NotifyIcon.ShowBalloonTip("HandyControl", "test", NotifyIconInfoType.None, "");
                    ic.SendNotify("提醒", timePicker.Text);
                }
                else
                {
                    HandyControl.Controls.Growl.Warning("格式不合法");
                }


            }
            catch (System.Exception)
            {


            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {

            Helper.PopMessageCenter("标题", "内容/n/r内容");

            //MessageBox.Show("内容/n/r内容", "标题", MessageBoxButton.OK, MessageBoxImage.Information);

            //MessageBox.Show(new MessageBoxInfo
            //{
            //    Message = "test",
            //    Caption = "test",
            //    Button = MessageBoxButton.YesNo,
            //    IconBrushKey = ResourceToken.AccentBrush,
            //    IconKey = ResourceToken.AskGeometry,
            //    StyleKey = "MessageBoxCustom"
            //});
        }


        //private void Test(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Application.Current.Dispatcher.Invoke(
        //        delegate {
        //            GetNetworkTime(inp.Text);
        //        });
        //    }
        //    catch (Exception ee)
        //    {
        //        MessageBox.Show(ee.Message);
        //    }
        //}

        //private void Test2(object sender, RoutedEventArgs e)
        //{
        //    set.Show();
        //    Helper.SetWindowTop("Reminder.Set");
        //}
    }
}
