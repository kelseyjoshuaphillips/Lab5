using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models
{
    public enum Question
    {
        Earth, Computer
    }
    public class Prediction
    {
        public int PredictionId { get; set; }
        [Required]
        [StringLength(50)]
        [Column("FileName")]
        [Display(Name ="File Name")]
        public string FileName { get; set; }
        [Required]
        [StringLength(100)]
        public string Url { get; set; }

        [Required]
        public Question Question { get; set; }

    }
}
