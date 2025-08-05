using System.Collections.ObjectModel;

namespace Reminder
{
    public class ReminderConfig
    {
        public string version { get; set; } = "1.0";
        public string extends { get; set; } = "";
        public ObservableCollection<ListDataModel> reminders { get; set; } = new ObservableCollection<ListDataModel>();
    }
}
