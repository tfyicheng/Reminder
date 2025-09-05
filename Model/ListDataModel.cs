using Newtonsoft.Json;
using Reminder;
using System;

[JsonObject(MemberSerialization.OptIn)]
public class ListDataModel : ViewModelBase
{
    public ListDataModel() : base(null) { }

    private int id;
    private string title;
    private string content;
    private string time;
    private int type;
    private int action;
    private int status;

    [JsonProperty]
    public int ID
    {
        get => id;
        set { id = value; OnPropertyChanged("ID"); }
    }

    [JsonProperty]
    public string Title
    {
        get => title;
        set { title = value; OnPropertyChanged("Title"); }
    }

    [JsonProperty]
    public string Content
    {
        get => content;
        set { content = value; OnPropertyChanged("Content"); }
    }

    [JsonProperty]
    public string Time
    {
        get => time;
        set { time = value; OnPropertyChanged("Time"); }
    }

    [JsonProperty]
    public int Type
    {
        get => type;
        set
        {
            type = value;
            OnPropertyChanged("Type");
        }
    }

    [JsonProperty]
    public int Action
    {
        get => action;
        set { action = value; OnPropertyChanged("Action"); }
    }

    [JsonProperty]
    public int Status
    {
        get => status;
        set
        {
            if (status != value)
            {
                if (value == 1)
                {
                    if (Helper.CheckListData(this))
                    {
                        status = value;
                        OnPropertyChanged("Status");
                    }
                }
                else
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
    }

    public DateTime? LastTriggeredTime { get; set; }

}
