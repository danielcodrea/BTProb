using System.ComponentModel.DataAnnotations;
using BT_DataModels;

namespace BTProb.Models
{
    public class JobModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The JobName value cannot exceed 50 characters. ")]
        public string JobName { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The JobType value cannot exceed 50 characters. ")]
        [EnumDataType(typeof(JobType))]
        public string JobType { get; set; }
    }
}
