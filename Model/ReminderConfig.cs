using System.Collections.ObjectModel;

namespace Reminder
{
    public class ReminderConfig
    {
        public string version { get; set; } = "1.8";
        public string extends { get; set; } = "";
        public SetModel SetModel { get; set; } = new SetModel();
        public ObservableCollection<ListDataModel> reminders { get; set; } = new ObservableCollection<ListDataModel>();
    }
}
