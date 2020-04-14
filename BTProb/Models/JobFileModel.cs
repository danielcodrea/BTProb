using System.ComponentModel.DataAnnotations;

namespace BTProb.Models
{
    public class JobFileModel : JobModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The FilePath value cannot exceed 100 characters. ")]
        public string FilePath { get; set; }
        //[Required]
        //[DataType(DataType.Upload)]
        //public IFormFile File{ get; set; }
    }
}
