using NtpTestDemo;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;
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


            ic = new myIcon();
            ic.Icon();


            (DataContext as ReminderModel).DataList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    // 获取刚添加的项
                    var newItem = e.NewItems[0]; // 就是插入的新对象

                    // 等待 UI 更新后滚动
                    dataGrid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        dataGrid.ScrollIntoView(newItem);
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                }
            };

            reminder.test();
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            StartProCheck();
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

            reminder.test();

            //Helper.PopMessageCenter("标题", "内容/n/r内容");

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



        //重复程序检查
        public void StartProCheck()
        {
            System.Diagnostics.Process[] myProcess = System.Diagnostics.Process.GetProcessesByName("Reminder");
            if (myProcess == null)
            {
                return;
            }

            if (myProcess.Length > 1)
            {
                MessageBoxResult result = MessageBox.Show("是否继续运行？", "检测到后台存在其他实例", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    Process proceMain = Process.GetCurrentProcess();
                    //Console.WriteLine(proceMain.ProcessName +"="+ proceMain.Id);
                    Process[] processes = Process.GetProcesses();
                    foreach (Process process in processes)//获取所有同名进程id
                    {
                        //Console.WriteLine(process.ProcessName+"="+process.Id);
                        if (process.ProcessName == "Reminder")
                        {
                            if (process.Id != proceMain.Id)//根据进程id删除所有除本进程外的所有相同进程
                            {
                                process.Kill();
                                return;
                            }
                        }
                    }
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
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
