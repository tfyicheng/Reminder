using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;

namespace Reminder
{
    public static class ReminderScheduler
    {
        private static Timer _timer;

        private static ObservableCollection<ListDataModel> _reminders;

        private static readonly double AllowedSeconds = 5;

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

            foreach (var reminder in _reminders)
            {
                if (reminder.Status != 1)
                    continue;

                if (!DateTime.TryParse(reminder.Time, out DateTime targetTime))
                    continue;

                // 根据提醒类型进行不同的判断逻辑
                switch (reminder.Type)
                {
                    case 1: // 一次性提醒
                            // 一次性提醒保持原有逻辑，因为它基于完整时间戳且只触发一次
                        if (now >= targetTime &&
                            (reminder.LastTriggeredTime == null ||
                             reminder.LastTriggeredTime.Value.Date != now.Date ||
                             Math.Abs((reminder.LastTriggeredTime.Value.TimeOfDay - targetTime.TimeOfDay).TotalSeconds) >= AllowedSeconds))
                        {
                            reminder.LastTriggeredTime = now;
                            reminder.Status = 0; // 执行一次后停止

                            if ((now - targetTime).TotalMinutes <= 5)
                            {
                                TriggerReminder(reminder);
                            }
                        }
                        break;

                    case 2: // 每日提醒
                            // 只比较时间部分，不比较日期部分
                        TimeSpan nowTime = now.TimeOfDay;
                        TimeSpan targetDailyTime = targetTime.TimeOfDay;

                        // 计算当前时间与目标时间的秒数差
                        double secondsDiff = (nowTime - targetDailyTime).TotalSeconds;

                        // 判断是否在允许的误差范围内
                        if (secondsDiff >= 0 && secondsDiff <= AllowedSeconds)
                        {
                            // 检查今天是否已经触发过
                            if (!IsTodayTriggered(reminder.ID, now.Date))
                            {
                                // 记录今天已触发
                                RecordTrigger(reminder.ID, now);
                                TriggerReminder(reminder);
                            }
                        }
                        break;

                    case 3: // 工作日提醒
                            // 首先检查是否是工作日
                        if (now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday)
                        {
                            // 只比较时间部分
                            TimeSpan nowWorkTime = now.TimeOfDay;
                            TimeSpan targetWorkTime = targetTime.TimeOfDay;

                            // 计算当前时间与目标时间的秒数差
                            double workSecondsDiff = (nowWorkTime - targetWorkTime).TotalSeconds;

                            // 判断是否在允许的误差范围内
                            if (workSecondsDiff >= 0 && workSecondsDiff <= AllowedSeconds)
                            {
                                // 检查今天是否已经触发过
                                if (!IsTodayTriggered(reminder.ID, now.Date))
                                {
                                    // 记录今天已触发
                                    RecordTrigger(reminder.ID, now);
                                    TriggerReminder(reminder);
                                }
                            }
                        }
                        break;

                    case 4: // 每月提醒
                            // 检查日期是否匹配（同一天）
                        if (now.Day == targetTime.Day)
                        {
                            // 只比较时间部分
                            TimeSpan nowMonthTime = now.TimeOfDay;
                            TimeSpan targetMonthTime = targetTime.TimeOfDay;

                            // 计算当前时间与目标时间的秒数差
                            double monthSecondsDiff = (nowMonthTime - targetMonthTime).TotalSeconds;

                            // 判断是否在允许的误差范围内
                            if (monthSecondsDiff >= 0 && monthSecondsDiff <= AllowedSeconds)
                            {
                                // 检查本月是否已经触发过
                                if (!IsMonthTriggered(reminder.ID, new DateTime(now.Year, now.Month, 1)))
                                {
                                    // 记录本月已触发
                                    RecordMonthlyTrigger(reminder.ID, now);
                                    TriggerReminder(reminder);
                                }
                            }
                        }
                        break;

                    case 5: // 间隔提醒（只取时分秒部分作为间隔时间）
                            // 获取目标时间的时分秒部分
                        TimeSpan intervalTime = targetTime.TimeOfDay;

                        // 如果没有上次触发时间，记录当前时间但不触发提醒
                        if (reminder.LastTriggeredTime == null)
                        {
                            reminder.LastTriggeredTime = now;
                            //Console.WriteLine(now + "-间隔提醒初始化-");
                        }
                        else
                        {
                            // 计算精确的时间差
                            TimeSpan elapsedTime = now - reminder.LastTriggeredTime.Value;

                            // 确保间隔时间至少为1秒
                            double minIntervalSeconds = Math.Max(1, intervalTime.TotalSeconds);

                            // 只有当经过的时间大于等于设定的间隔时间时才触发
                            if (elapsedTime.TotalSeconds >= minIntervalSeconds)
                            {
                                //Console.WriteLine(now + "-触发间隔提醒-实际间隔：" + elapsedTime.TotalSeconds + "秒");

                                // 更新上次触发时间为当前时间
                                reminder.LastTriggeredTime = now;

                                // 触发提醒
                                TriggerReminder(reminder);
                            }
                        }
                        break;

                    case 6: // 每年提醒（取月日时分秒）
                            // 检查月份和日期是否匹配
                        if (now.Month == targetTime.Month && now.Day == targetTime.Day)
                        {
                            // 只比较时间部分
                            TimeSpan nowYearTime = now.TimeOfDay;
                            TimeSpan targetYearTime = targetTime.TimeOfDay;

                            // 计算当前时间与目标时间的秒数差
                            double yearSecondsDiff = (nowYearTime - targetYearTime).TotalSeconds;

                            // 判断是否在允许的误差范围内
                            if (yearSecondsDiff >= 0 && yearSecondsDiff <= AllowedSeconds)
                            {
                                // 检查今年是否已经触发过
                                if (!IsYearTriggered(reminder.ID, now.Year))
                                {
                                    //Console.WriteLine(now + "-触发每年提醒-");
                                    // 记录今年已触发
                                    RecordYearlyTrigger(reminder.ID, now.Year);
                                    TriggerReminder(reminder);
                                }
                            }
                        }
                        break;
                }
            }
        }


        // 使用字典来记录每个提醒的触发日期，避免使用LastTriggeredTime
        private static Dictionary<int, HashSet<DateTime>> _dailyTriggerRecords = new Dictionary<int, HashSet<DateTime>>();
        private static Dictionary<int, HashSet<DateTime>> _monthlyTriggerRecords = new Dictionary<int, HashSet<DateTime>>();
        private static Dictionary<int, HashSet<int>> _yearlyTriggerRecords = new Dictionary<int, HashSet<int>>(); // 新增：记录年度触发

        // 检查今天是否已经触发过
        private static bool IsTodayTriggered(int reminderId, DateTime date)
        {
            if (!_dailyTriggerRecords.ContainsKey(reminderId))
                return false;

            return _dailyTriggerRecords[reminderId].Contains(date.Date);
        }

        // 记录触发
        private static void RecordTrigger(int reminderId, DateTime triggerTime)
        {
            if (!_dailyTriggerRecords.ContainsKey(reminderId))
                _dailyTriggerRecords[reminderId] = new HashSet<DateTime>();

            _dailyTriggerRecords[reminderId].Add(triggerTime.Date);

            // 清理旧记录（可选，保留最近30天的记录）
            CleanupOldRecords();
        }

        // 检查本月是否已经触发过
        private static bool IsMonthTriggered(int reminderId, DateTime monthStart)
        {
            if (!_monthlyTriggerRecords.ContainsKey(reminderId))
                return false;

            return _monthlyTriggerRecords[reminderId].Contains(monthStart);
        }

        // 记录月度触发
        private static void RecordMonthlyTrigger(int reminderId, DateTime triggerTime)
        {
            if (!_monthlyTriggerRecords.ContainsKey(reminderId))
                _monthlyTriggerRecords[reminderId] = new HashSet<DateTime>();

            // 记录月份的第一天
            DateTime monthStart = new DateTime(triggerTime.Year, triggerTime.Month, 1);
            _monthlyTriggerRecords[reminderId].Add(monthStart);

            // 清理旧记录（可选，保留最近12个月的记录）
            CleanupOldMonthlyRecords();
        }

        // 清理旧的日期记录
        private static void CleanupOldRecords()
        {
            DateTime cutoffDate = DateTime.Now.AddDays(-30);

            foreach (var reminderId in _dailyTriggerRecords.Keys.ToList())
            {
                _dailyTriggerRecords[reminderId] = new HashSet<DateTime>(
                    _dailyTriggerRecords[reminderId].Where(date => date >= cutoffDate)
                );
            }
        }

        // 清理旧的月度记录
        private static void CleanupOldMonthlyRecords()
        {
            DateTime cutoffDate = DateTime.Now.AddMonths(-12);

            foreach (var reminderId in _monthlyTriggerRecords.Keys.ToList())
            {
                _monthlyTriggerRecords[reminderId] = new HashSet<DateTime>(
                    _monthlyTriggerRecords[reminderId].Where(date => date >= cutoffDate)
                );
            }
        }


        // 新增：检查今年是否已经触发过
        private static bool IsYearTriggered(int reminderId, int year)
        {
            if (!_yearlyTriggerRecords.ContainsKey(reminderId))
                return false;

            return _yearlyTriggerRecords[reminderId].Contains(year);
        }

        // 新增：记录年度触发
        private static void RecordYearlyTrigger(int reminderId, int year)
        {
            if (!_yearlyTriggerRecords.ContainsKey(reminderId))
                _yearlyTriggerRecords[reminderId] = new HashSet<int>();

            _yearlyTriggerRecords[reminderId].Add(year);

            // 清理旧记录（可选，保留最近5年的记录）
            CleanupOldYearlyRecords();
        }

        // 新增：清理旧的年度记录
        private static void CleanupOldYearlyRecords()
        {
            int cutoffYear = DateTime.Now.Year - 5;

            foreach (var reminderId in _yearlyTriggerRecords.Keys.ToList())
            {
                _yearlyTriggerRecords[reminderId] = new HashSet<int>(
                    _yearlyTriggerRecords[reminderId].Where(year => year >= cutoffYear)
                );
            }
        }

        private static void TriggerReminder(ListDataModel reminder)
        {
            if (ReminderStorage.configModel.SetModel.Run1)
            {
                ReminderSoundHelper.PlayReminderSound(ReminderStorage.configModel.SetModel);
            }
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
                case 4:
                    ReminderActionHelper.webhook(reminder);
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

        public static void webhook(ListDataModel reminder)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(async delegate
            {
                var replacements = new Dictionary<string, string>
                {
                    { "标题", reminder.Title },
                    { "内容",reminder.Content },
                    { "时间",reminder.Time }
                };
                string result = await Server.SendAsync(ReminderStorage.configModel.SetModel.RequestType0, ReminderStorage.configModel.SetModel.RequestApi0, ReminderStorage.configModel.SetModel.RequestBody0, replacements);
            });
        }

    }
}
