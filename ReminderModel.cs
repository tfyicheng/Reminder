using System.Collections.ObjectModel;

namespace Reminder
{
    public class ReminderModel : ViewModelBase
    {
        private ObservableCollection<ListDataModel> datalist;//记录列表


        private int editStatus;//编辑框状态  0 初始化空白    1 获取编辑内容未开始编辑     2 编辑状态   


        public ObservableCollection<ListDataModel> DataList
        {
            get => datalist;

            set
            {
                if (datalist != value)
                {
                    datalist = value;
                    OnPropertyChanged("DataList");
                }
            }
        }

        public int EditStatus
        {
            get => editStatus;

            set
            {
                if (editStatus != value)
                {
                    editStatus = value;
                    OnPropertyChanged("EditStatus");
                }
            }
        }

        public ReminderModel() : base(null)
        {
            datalist = new ObservableCollection<ListDataModel>();
        }

        public void test()
        {
            for (int i = 0; i < 10; i++)
            {
                datalist.Add(new ListDataModel()
                {
                    ID = i,
                    Title = $"{i}标题",
                    Type = i,
                    Status = i,
                    Time = $"{i}contTimeent",
                    Content = $"{i}content",
                });
            }
        }
    }
}
