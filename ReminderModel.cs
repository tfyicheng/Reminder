using HandyControl.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;
namespace Reminder
{
    public class ReminderModel : ViewModelBase
    {
        private ObservableCollection<ListDataModel> datalist;//记录列表


        private int editStatus; //编辑框状态  0 初始化空白    1 获取编辑内容未开始编辑     2 编辑状态   

        private ListDataModel editModel;//编辑内容对象

        private ListDataModel _selectedItem;//选中对象

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

        public ListDataModel EditModel
        {
            get => editModel;

            set
            {
                if (editModel != value)
                {
                    editModel = value;
                    OnPropertyChanged("EditModel");
                }
            }
        }

        public ListDataModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");

                // 处理选中逻辑
                if (value != null)
                {
                    EditModel = value;
                    EditStatus = 1;
                }
            }
        }

        public ReminderModel() : base(null)
        {
            datalist = new ObservableCollection<ListDataModel>();
            editStatus = 0;
            NewSet();
        }

        #region addcmd

        protected RelayCommand addcmd;
        public RelayCommand AddCmd
        {
            get
            {
                if (addcmd == null)
                {
                    addcmd = new RelayCommand(param => this.AddSet(), param => this.AddCmdCanExecuted());
                }
                return addcmd;
            }
        }

        public void AddSet()
        {
            //校验对象合法?
            if (Helper.CheckListData(EditModel))
            {
                //添加
                DataList.Insert(0, EditModel);

                NewSet();
            }

        }

        public bool AddCmdCanExecuted()
        {
            return true;
        }
        #endregion

        #region newcmd

        protected RelayCommand newcmd;
        public RelayCommand NewCmd
        {
            get
            {
                if (newcmd == null)
                {
                    newcmd = new RelayCommand(param => this.NewSet(), param => this.NewCmdCanExecuted());
                }
                return newcmd;
            }
        }

        public void NewSet()
        {
            EditStatus = 0;
            EditModel = new ListDataModel()
            {
                ID = DataList.Count + 1,
                Title = "",
                Content = "",
                Time = "",
                Type = 1,
                Action = 1,
                Status = 1,
            };

        }

        public bool NewCmdCanExecuted()
        {
            return true;
        }
        #endregion

        #region editcmd

        protected RelayCommand editcmd;
        public RelayCommand EditCmd
        {
            get
            {
                if (editcmd == null)
                {
                    editcmd = new RelayCommand(param => this.EditSet(), param => this.EditCmdCanExecuted());
                }
                return editcmd;
            }
        }

        public void EditSet()
        {
            EditStatus = 2;
        }

        public bool EditCmdCanExecuted()
        {
            return true;
        }
        #endregion

        #region cancelcmd

        protected RelayCommand cancelcmd;
        public RelayCommand CancelCmd
        {
            get
            {
                if (cancelcmd == null)
                {
                    cancelcmd = new RelayCommand(param => this.CancelSet(), param => this.CancelCmdCanExecuted());
                }
                return cancelcmd;
            }
        }

        public void CancelSet()
        {
            EditStatus = 1;
        }

        public bool CancelCmdCanExecuted()
        {
            return true;
        }
        #endregion

        #region deletcmd

        protected RelayCommand deletcmd;
        public RelayCommand DeletCmd
        {
            get
            {
                if (deletcmd == null)
                {
                    deletcmd = new RelayCommand(param => this.DeletSet(), param => this.DeletCmdCanExecuted());
                }
                return deletcmd;
            }
        }

        public void DeletSet()
        {
            MessageBoxResult result = MessageBox.Show("是否删除提醒？", "确认操作", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                bool isRemoved = DataList.Remove(EditModel);

                if (isRemoved)
                {
                    Growl.Info("删除成功");
                }
                else
                {
                    Growl.Error("未找到该对象（可能已被删除或引用不匹配）");
                }
                NewSet();
            }
        }

        public bool DeletCmdCanExecuted()
        {
            return true;
        }
        #endregion
        public void test()
        {
            var ld = new ListDataModel()
            {
                ID = 1,
                Title = $"标题",
                Type = 1,
                Status = 0,
                Action = 1,
                Time = $"2021-09-20 21:20:21",
                Content = $"content",
            };
            var ld2 = new ListDataModel()
            {
                ID = 2,
                Title = $"标题标题标题标题标题",
                Type = 2,
                Status = 1,
                Action = 2,
                Time = $"04:20:20",
                Content = $"contencontentcontentcontentcontentcontentcontentcontentcontentcontentcontentcontentcontentt",
            };
            var ld3 = new ListDataModel()
            {
                ID = 3,
                Title = $"333标题",
                Type = 3,
                Status = 1,
                Action = 3,
                Time = $"2025-08-20 20:20:23",
                Content = $"3content",
            };

            var ld4 = new ListDataModel()
            {
                ID = 44,
                Title = $"444444444444444444444标题",
                Type = 4,
                Status = 0,
                Action = 3,
                Time = $"2021-07-21 09:12:20",
                Content = $"3content",
            };

            var ld5 = new ListDataModel()
            {
                ID = 999,
                Title = $"55yyyyy标题",
                Type = 5,
                Status = 5,
                Action = 5,
                Time = $"",
                Content = $"",
            };
            datalist.Add(ld);
            datalist.Add(ld2);
            datalist.Add(ld3);
            datalist.Add(ld4);
            datalist.Add(ld5);

        }
    }
}
