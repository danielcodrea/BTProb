using System.ComponentModel;

namespace BT_DataModels
{
    public enum JobType
    {
        Unspecified = 0,
        [Description("Notification")]
        Notification = 1,
        PayCheck = 2,
        FileJob = 3
    }
}
