using System;

namespace Reminder
{
    public class ListDataModel : ViewModelBase
    {
        public ListDataModel() : base(null)
        {
        }

        private static int id;//id

        private static String title;//标题

        private static String content;//内容

        private static String time; //时间

        private static int type;//循环类型

        private static int action;//提醒动作

        private static int status;//状态

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
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

    }
}
