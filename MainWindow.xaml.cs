using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MessageBox = HandyControl.Controls.MessageBox;
namespace Reminder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = Helper.reminder = new ReminderModel(); ;

            Helper.ic.Icon();

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
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            StartProCheck();
            Helper.reminder.DataList = ReminderStorage.LoadReminders()?.reminders ?? new ObservableCollection<ListDataModel>();
            ReminderScheduler.Start(Helper.reminder.DataList);
            Helper.reminder.InitDataView();
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

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true; // 标记为已处理

                this.Close();
            }
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //Helper.setwindow.Show();
            //Helper.SetWindowTop("Reminder.SetWindow");


            Helper.setwindow?.Close(); // 关闭旧窗口

            Helper.setwindow = new SetWindow();
            Helper.setwindow.Owner = this;
            Helper.setwindow.Show();
        }
    }
}
