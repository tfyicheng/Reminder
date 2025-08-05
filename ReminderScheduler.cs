using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;

namespace Reminder
{
    public static class ReminderScheduler
    {
        private static Timer _timer;
        private static ObservableCollection<ListDataModel> _reminders;

        public static void Start(ObservableCollection<ListDataModel> reminders)
        {
            _reminders = reminders;
            _timer = new Timer(1000); // 每秒执行一次
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Start();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            double allowedSeconds = 5; // 允许 5 秒误差

            foreach (var reminder in _reminders)
            {
                if (reminder.Status != 1)
                    continue;

                if (!DateTime.TryParse(reminder.Time, out DateTime targetTime))
                    continue;

                TimeSpan delta = now - targetTime;

                switch (reminder.Type)
                {
                    case 1: // 一次性
                        if (now >= targetTime &&
                            (reminder.LastTriggeredTime == null ||
                             reminder.LastTriggeredTime.Value.Date != now.Date ||
                             reminder.LastTriggeredTime.Value.TimeOfDay != targetTime.TimeOfDay))
                        {
                            reminder.LastTriggeredTime = now;
                            reminder.Status = 0; // 停止

                            if (delta.TotalMinutes <= 5)
                            {
                                TriggerReminder(reminder);
                            }


                        }
                        break;

                    case 2: // 每日
                        if (delta.TotalSeconds >= 0 && delta.TotalSeconds <= allowedSeconds &&
                            (reminder.LastTriggeredTime == null ||
                             (now - reminder.LastTriggeredTime.Value).TotalSeconds > allowedSeconds))
                        {
                            reminder.LastTriggeredTime = now;
                            TriggerReminder(reminder);

                        }
                        break;

                    case 3: // 工作日
                        if (now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday)
                        {
                            if (delta.TotalSeconds >= 0 && delta.TotalSeconds <= allowedSeconds &&
                                (reminder.LastTriggeredTime == null ||
                                 (now - reminder.LastTriggeredTime.Value).TotalSeconds > allowedSeconds))
                            {
                                reminder.LastTriggeredTime = now;
                                TriggerReminder(reminder);

                            }
                        }
                        break;

                    case 4: // 每月
                        if (now.Day == targetTime.Day)
                        {
                            if (delta.TotalSeconds >= 0 && delta.TotalSeconds <= allowedSeconds &&
                                (reminder.LastTriggeredTime == null ||
                                 (now - reminder.LastTriggeredTime.Value).TotalSeconds > allowedSeconds))
                            {
                                reminder.LastTriggeredTime = now;
                                TriggerReminder(reminder);
                            }
                        }
                        break;
                }
            }
        }



        private static bool IsTriggeredToday(ListDataModel reminder, DateTime now)
        {
            return reminder.LastTriggeredTime != null &&
                   reminder.LastTriggeredTime.Value.Date == now.Date;
        }


        private static void TriggerReminder(ListDataModel reminder)
        {
            switch (reminder.Action)
            {
                case 1:
                    ReminderActionHelper.ShowSystemNotification(reminder);
                    break;
                case 2:
                    ReminderActionHelper.ShowBottomRightPopup(reminder);
                    break;
                case 3:
                    ReminderActionHelper.ShowCenterPopup(reminder);
                    break;
            }
        }
    }

    public static class ReminderActionHelper
    {
        public static void ShowSystemNotification(ListDataModel reminder)
        {
            Helper.ic.SendNotify(reminder.Title, reminder.Content);
        }

        public static void ShowBottomRightPopup(ListDataModel reminder)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Notification.Show(new AppNotification(reminder.Title, reminder.Content), ShowAnimation.HorizontalMove, true);
            });
        }

        public static void ShowCenterPopup(ListDataModel reminder)
        {
            Helper.PopMessageCenter(reminder.Title, reminder.Content);
        }
    }
}
