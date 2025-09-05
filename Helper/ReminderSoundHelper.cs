using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Reminder
{
    public static class ReminderSoundHelper
    {
        // 按需修改成你的命名空间和文件名
        private const string EmbeddedSoundResource = "Reminder.default.mp3";

        private static readonly MediaPlayer _player = new MediaPlayer();
        private static string _cachedEmbeddedPath; // 缓存解包后的临时文件，避免每次重写
        /// <summary>
        /// 播放提示音：优先播放 configModel.MusicPath，否则播放嵌入的默认音频
        /// </summary>
        public static void PlayReminderSound(SetModel configModel)
        {
            try
            {
                // 1) 选路径：优先外部配置
                string path = null;
                if (configModel != null &&
                    !string.IsNullOrWhiteSpace(configModel.MusicPath) &&
                    File.Exists(configModel.MusicPath))
                {
                    path = configModel.MusicPath;
                }
                else
                {
                    // 2) 解包嵌入资源到 Temp，缓存路径
                    if (_cachedEmbeddedPath == null)
                    {
                        _cachedEmbeddedPath = ExtractEmbeddedToTemp(EmbeddedSoundResource, "reminder_default_embedded.mp3");
                    }
                    path = _cachedEmbeddedPath;
                }

                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return;

                // 3) MediaPlayer 必须在 UI 线程操作
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var player = new MediaPlayer();
                        player.Open(new Uri(path, UriKind.Absolute));
                        player.Volume = 1.0;
                        player.Play();

                        // 播放完自动关闭
                        player.MediaEnded += (s, e) =>
                        {
                            player.Close();
                        };
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("播放提示音失败(UI): " + ex);
                        HandyControl.Controls.Growl.Warning("播放提示音失败(UI): " + ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("播放提示音失败: " + ex);
                HandyControl.Controls.Growl.Warning("播放提示音失败: " + ex);
            }
        }

        /// <summary>
        /// 从嵌入资源解包到临时文件（返回绝对路径）
        /// </summary>
        private static string ExtractEmbeddedToTemp(string resourceName, string fileName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    // 调试用：打印出所有嵌入资源名，帮你确认 resourceName 是否正确
#if DEBUG
                    DumpResourceNames();
#endif
                    return null;
                }

                string tempPath = Path.Combine(Path.GetTempPath(), fileName);
                FileStream fs = null;
                try
                {
                    fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    stream.CopyTo(fs);
                }
                finally
                {
                    if (fs != null) fs.Dispose();
                    if (stream != null) stream.Dispose();
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("解包嵌入音频失败: " + ex);
                return null;
            }
        }

        [Conditional("DEBUG")]
        private static void DumpResourceNames()
        {
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (var n in names)
                Debug.WriteLine("Embedded resource: " + n);
        }
    }
}
