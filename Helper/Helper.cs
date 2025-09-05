using HandyControl.Controls;
using Microsoft.Win32;
using NtpTestDemo;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MessageBox = HandyControl.Controls.MessageBox;
namespace Reminder
{
    public static class Helper
    {
        public static SetWindow setwindow { set; get; }
        public static ReminderModel reminder { set; get; }


        public static myIcon ic = new myIcon();

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        /// <summary>
        /// 设置窗体前置
        /// </summary>
        /// <param name="windowname"></param>
        public static void SetWindowTop(string windowname)
        {
            try
            {
                foreach (var item in Application.Current.Windows)
                {
                    //Console.WriteLine(item.ToString());
                    if (item.ToString() == windowname)
                    {
                        //Console.WriteLine("设置主窗体");
                        System.Windows.Window w = (System.Windows.Window)item;
                        w.WindowState = System.Windows.WindowState.Normal;
                        Helper.SetWindowToForegroundWithAttachThreadInput(w);
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// 设置窗体激活前置
        /// </summary>
        /// <param name="window"></param>
        public static void SetWindowToForegroundWithAttachThreadInput(System.Windows.Window window)
        {
            var interopHelper = new WindowInteropHelper(window);
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindow = GetForegroundWindow();
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);

            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);

            window.Show();
            window.Activate();
            // 去掉和其他线程的输入链接
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);
            // 用于踢掉其他的在上层的窗口
            window.Topmost = true;
            window.Topmost = false;
        }


        //注册表方式

        // 获取应用程序的名称
        private static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;

        // 获取应用程序的可执行文件路径
        private static readonly string AppPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        // 注册表启动项的路径
        private const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// 设置开机自启动
        /// </summary>
        /// <param name="isEnabled">true为开启，false为关闭</param>
        public static void SetAutoStart(bool isEnabled)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, true))
                {
                    if (key == null) throw new Exception("无法打开注册表启动项。");

                    if (isEnabled)
                    {
                        // 加上引号避免路径有空格时失效
                        key.SetValue(AppName, $"\"{AppPath}\"");
                    }
                    else
                    {
                        if (key.GetValue(AppName) != null)
                            key.DeleteValue(AppName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置开机启动失败: {ex.Message}");
                HandyControl.Controls.Growl.Warning($"设置开机启动失败: {ex.Message}");
            }
        }




        public static bool IsValidDateTime(string dateTimeStr)
        {
            return DateTime.TryParseExact(
                dateTimeStr,
                "yyyy/M/d H:mm:ss",  // 自动匹配 M/d 可为单/双位
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _
            );
        }


        //弹窗
        public static void PopMessageCenter(String title, String content)
        {
            MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //检查提醒对象
        public static bool CheckListData(ListDataModel listDataModel)
        {
            if (listDataModel == null)
            {
                Growl.Warning("数据对象为空。");
                return false;
            }

            // 标题和内容不能为空
            if (string.IsNullOrWhiteSpace(listDataModel.Title) && string.IsNullOrWhiteSpace(listDataModel.Content))
            {
                Growl.Warning("标题或内容其中一项不能为空。");
                return false;
            }

            //if (string.IsNullOrWhiteSpace(listDataModel.Content))
            //{
            //    Growl.Warning("内容不能为空。");
            //    return false;
            //}

            // 校验时间字段
            string timeStr = listDataModel.Time?.Trim();
            if (string.IsNullOrWhiteSpace(timeStr))
            {
                Growl.Warning("时间不能为空。");
                return false;
            }

            DateTime now = DateTime.Now;
            DateTime parsedTime;

            switch (listDataModel.Type)
            {
                case 1: // 关闭：需要完整时间并且是未来时间
                    if (!DateTime.TryParseExact(timeStr, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out parsedTime))
                    {
                        Growl.Warning("一次性类型要求时间格式为 yyyy-MM-dd HH:mm:ss。");
                        return false;
                    }
                    if (parsedTime <= now)
                    {
                        Growl.Warning($"一次性类型要求时间在未来，当前时间为 {now}，输入时间为 {parsedTime}。");
                        return false;
                    }
                    break;

                //case 2: // 每日：只校验时间格式（时分秒）
                //case 3: // 工作日：只校验时间格式（时分秒）
                //    if (!DateTime.TryParseExact(timeStr, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out _))
                //    {
                //        Growl.Warning("错误：每日或工作日类型要求时间格式为 HH:mm:ss。");
                //        return false;
                //    }
                //    break;

                case 4: // 每月：校验 yyyy-MM-dd HH:mm:ss，但只判断月日时分秒是否合理
                    if (!DateTime.TryParseExact(timeStr, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out parsedTime))
                    {
                        Growl.Warning("每月类型要求时间格式为 yyyy-MM-dd HH:mm:ss。");
                        return false;
                    }

                    if (parsedTime.Day < 1 || parsedTime.Day > 31)
                    {
                        Growl.Warning("每月类型中日期不合法（1-31）。");
                        return false;
                    }

                    // 时间部分校验可以认为已经在 Parse 时完成
                    break;

                    //default:
                    //    Growl.Warning("错误：未知的循环类型。");
                    //    return false;
            }

            // 通过所有校验
            return true;
        }

        private static readonly Random _random = new Random();
        public static int GenerateRandomID()
        {
            return _random.Next(0, 1000);
        }


        public static string GetValue(string key)
        {
            switch (key)
            {
                case "标题":
                    return "";
                default:
                    return $"[{key}?]";
            }
        }

    }
}
