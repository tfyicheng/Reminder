using System.Windows;

namespace Reminder
{
    /// <summary>
    /// SetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetWindow : Window
    {
        public SetWindow()
        {
            InitializeComponent();
            this.DataContext = ReminderStorage.LoadReminders()?.SetModel ?? new SetModel();
        }

    }
}
