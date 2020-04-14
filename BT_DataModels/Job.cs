using System;

namespace BT_DataModels
{
    public class Job
    {
        public int ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public JobType Type { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public bool? Processed { get; set; }
        public bool FileAttached { get; set; }
        public string File { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
