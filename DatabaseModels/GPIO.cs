using System.ComponentModel.DataAnnotations;

namespace AVC.DatabaseModels
{
    public class GPIO
    {
        [Required]
        public virtual int port { get; set; }
        [Required]
        public virtual int value { get; set; }
        public virtual string name { get; set; }
    }
}