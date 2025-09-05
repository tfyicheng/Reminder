using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Reminder
{
    public static class ReminderStorage
    {
        private static readonly string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static ReminderConfig configModel = new ReminderConfig();

        public static ReminderConfig LoadReminders()
        {
            try
            {
                if (!File.Exists(configFile))
                {
                    Console.WriteLine("config.json 不存在，正在创建默认文件。");

                    var defaultConfig = new ReminderConfig();
                    SaveConfig(defaultConfig); // 创建空文件
                    return defaultConfig;
                }

                string json = File.ReadAllText(configFile);
                var config = JsonConvert.DeserializeObject<ReminderConfig>(json);
                Console.WriteLine("加载数据：" + config?.reminders.Count ?? json);
                //return config?.reminders ?? new ObservableCollection<ListDataModel>();
                configModel = config;
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine("加载提醒数据时发生错误：" + ex.Message);
                return new ReminderConfig();
            }
        }

        public static void SaveReminders(ObservableCollection<ListDataModel> reminders)
        {
            var config = LoadFullConfig();
            config.reminders = reminders;
            SaveConfig(config);
        }

        public static void SaveSet(SetModel set)
        {
            var config = LoadFullConfig();
            config.SetModel = set;
            SaveConfig(config);
        }

        public static void AddReminder(ListDataModel reminder)
        {
            var config = LoadFullConfig();
            config.reminders.Add(reminder);
            SaveConfig(config);
        }

        public static void DeleteReminder(int id)
        {
            var config = LoadFullConfig();
            var target = config.reminders.FirstOrDefault(r => r.ID == id);
            if (target != null)
            {
                config.reminders.Remove(target);
                SaveConfig(config);
                Console.WriteLine($"已删除提醒 ID={id}");
            }
            else
            {
                Console.WriteLine($"未找到 ID={id} 的提醒，无法删除。");
            }
        }

        public static void UpdateReminder(ListDataModel updatedReminder)
        {
            var config = LoadFullConfig();
            var index = config.reminders.ToList().FindIndex(r => r.ID == updatedReminder.ID);
            if (index >= 0)
            {
                config.reminders[index] = updatedReminder;
                SaveConfig(config);
                Console.WriteLine($"已更新提醒 ID={updatedReminder.ID}-{updatedReminder.Title}");
            }
            else
            {
                Console.WriteLine($"未找到 ID={updatedReminder.ID} 的提醒，无法更新。");
            }
        }

        private static ReminderConfig LoadFullConfig()
        {
            try
            {
                if (!File.Exists(configFile))
                {
                    return new ReminderConfig();
                }

                string json = File.ReadAllText(configFile);
                return JsonConvert.DeserializeObject<ReminderConfig>(json) ?? new ReminderConfig();
            }
            catch
            {
                return new ReminderConfig();
            }
        }

        private static void SaveConfig(ReminderConfig config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configFile, json);
        }
    }
}
