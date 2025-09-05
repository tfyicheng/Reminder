using HandyControl.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using MessageBox = HandyControl.Controls.MessageBox;
namespace Reminder
{
    public class ReminderModel : ViewModelBase
    {
        private ObservableCollection<ListDataModel> datalist;//记录列表

        public ICollectionView FilteredView { get; set; }


        private int editStatus; //编辑框状态  0 初始化空白    1 获取编辑内容未开始编辑     2 编辑状态   

        private ListDataModel editModel;//编辑内容对象

        private ListDataModel _selectedItem;//选中对象


        private int selectedType = 0;
        public int SelectedType
        {
            get => selectedType;
            set
            {
                if (selectedType != value)
                {
                    selectedType = value;
                    FilteredView.Refresh();
                    OnPropertyChanged("SelectedType");
                }
            }
        }

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
                    ReminderStorage.UpdateReminder(value);
                    EditModel = value;
                    EditStatus = 1;
                }
            }
        }

        public ReminderModel() : base(null)
        {
            datalist = new ObservableCollection<ListDataModel>();
            editStatus = 0;
            InitModel();
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
                //补全内容
                if (string.IsNullOrWhiteSpace(EditModel.Title))
                {
                    EditModel.Title = EditModel.Content;
                };
                if (string.IsNullOrWhiteSpace(EditModel.Content))
                {
                    EditModel.Content = EditModel.Title;
                };

                //添加
                DataList.Insert(0, EditModel);

                ReminderStorage.AddReminder(EditModel);

                InitModel();

                SelectedItem = null;
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
            if (EditModel != null)
            {
                ReminderStorage.UpdateReminder(EditModel);
            }

            InitModel();
            SelectedItem = null;

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
                    ReminderStorage.DeleteReminder(EditModel.ID);
                }
                else
                {
                    Growl.Warning("未找到该对象（可能已被删除或引用不匹配）");
                }
                InitModel();
                EditStatus = 0;
            }
        }

        public bool DeletCmdCanExecuted()
        {
            return true;
        }
        #endregion

        public void InitModel()
        {
            EditModel = new ListDataModel()
            {
                ID = Helper.GenerateRandomID(),
                Title = "",
                Content = "",
                Time = "",
                Type = 1,
                Action = 1,
                Status = 0,
            };
        }

        public void InitDataView()
        {
            FilteredView = CollectionViewSource.GetDefaultView(datalist);
            FilteredView.Filter = o =>
            {
                if (SelectedType == 0) return true; // 0 表示全部
                return ((ListDataModel)o).Type == SelectedType;
            };
        }

        public void SaveModel()
        {
            ReminderStorage.SaveReminders(DataList);
        }
    }
}
