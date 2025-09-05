using Microsoft.Win32;
using Newtonsoft.Json;
using System;

namespace Reminder
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SetModel : ViewModelBase
    {
        public SetModel() : base(null)
        {
        }
        public String[] requestTypes { get; set; } = new String[] { "Get", "Post" };

        private string requestApi0;
        private string requestType0;
        private string requestBody0;
        private string musicPath;
        private bool run0;
        private bool run1;

        [JsonProperty]
        public string MusicPath
        {
            get => musicPath;
            set
            {

                musicPath = value;
                OnPropertyChanged("MusicPath");
            }
        }

        [JsonProperty]
        public string RequestApi0
        {
            get => requestApi0;
            set { requestApi0 = value; OnPropertyChanged(nameof(RequestApi0)); }
        }

        [JsonProperty]
        public string RequestType0
        {
            get => requestType0;
            set { requestType0 = value; OnPropertyChanged(nameof(RequestType0)); }
        }

        [JsonProperty]
        public string RequestBody0
        {
            get => requestBody0;
            set { requestBody0 = value; OnPropertyChanged(nameof(RequestBody0)); }
        }

        [JsonProperty]
        public bool Run0
        {
            get => run0;
            set
            {
                if (value)
                {
                    Helper.SetAutoStart(true);
                }
                else
                {
                    Helper.SetAutoStart(false);
                }
                run0 = value;
                OnPropertyChanged(nameof(Run0));
            }
        }

        [JsonProperty]
        public bool Run1
        {
            get => run1;
            set { run1 = value; OnPropertyChanged(nameof(Run1)); }
        }

        #region sureCmd

        protected RelayCommand sureCmd;
        public RelayCommand SureCmd
        {
            get
            {
                if (sureCmd == null)
                {
                    sureCmd = new RelayCommand(param => this.Sure(), param => this.SureSetCanExecuted());
                }
                return sureCmd;
            }
        }

        public void Sure()
        {
            Helper.setwindow.Hide();
            ReminderStorage.SaveSet(this);
        }

        public bool SureSetCanExecuted()
        {
            return true;
        }
        #endregion


        #region testCmd

        protected RelayCommand testCmd;
        public RelayCommand TestCmd
        {
            get
            {
                if (testCmd == null)
                {
                    testCmd = new RelayCommand(param => this.Test(), param => this.TestCanExecuted());
                }
                return testCmd;
            }
        }

        async public void Test()
        {
            if (String.IsNullOrEmpty(RequestType0) || String.IsNullOrEmpty(RequestApi0))
            {
                HandyControl.Controls.Growl.Warning("请先设置请求配置");
                return;
            }
            string result = await Server.SendAsync(RequestType0, RequestApi0, RequestBody0);
            HandyControl.Controls.Growl.Info(result);
        }

        public bool TestCanExecuted()
        {
            return true;
        }
        #endregion

        #region GetPathCommand

        protected RelayCommand getPathCommand;
        public RelayCommand GetPathCommand
        {
            get
            {
                if (getPathCommand == null)
                {
                    getPathCommand = new RelayCommand(param => this.GetPath(), param => this.getPathCanExecuted());
                }
                return getPathCommand;
            }
        }

        private void GetPath()
        {
            var dialog = new OpenFileDialog
            {
                Title = "选择文件",
                Filter = "音乐文件 (*.mp3;*.wav;*.wma)|*.mp3;*.wav;*.wma",
                CheckFileExists = true,
                Multiselect = false
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                MusicPath = dialog.FileName;
            }
        }

        public bool getPathCanExecuted()
        {
            return true;
        }
        #endregion



    }
}
