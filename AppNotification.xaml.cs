using System;
using System.Windows.Controls;

namespace Reminder
{
    /// <summary>
    /// AppNotification.xaml 的交互逻辑
    /// </summary>
    public partial class AppNotification : UserControl
    {
        public AppNotification(String t, String c)
        {
            InitializeComponent();
            title.Text = t;
            content.Text = c;
        }
    }
}
