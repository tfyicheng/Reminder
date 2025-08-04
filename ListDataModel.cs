using System;

namespace Reminder
{
    public class ListDataModel : ViewModelBase
    {
        public ListDataModel() : base(null)
        {
        }

        private int id;//id

        private String title;//标题

        private String content;//内容

        private String time; //时间

        private int type;//循环类型  1关闭  2每日  3工作日  4每月

        private int action;//提醒动作 1系统通知  2右下角弹窗  3中间弹窗

        private int status;//状态 0停止 1活动

        public int ID
        {
            get => id;

            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        public String Title
        {
            get => title;

            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public String Content
        {
            get => content;

            set
            {
                if (content != value)
                {
                    content = value;
                    OnPropertyChanged("Content");
                }
            }
        }


        public String Time
        {
            get => time;

            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        public int Type
        {
            get => type;

            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public int Action
        {
            get => action;

            set
            {
                if (action != value)
                {
                    action = value;
                    OnPropertyChanged("Action");
                }
            }
        }

        public int Status
        {
            get => status;

            set
            {
                if (status != value && Helper.CheckListData(this))
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

    }
}
