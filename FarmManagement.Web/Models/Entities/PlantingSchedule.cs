using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.Web.Models.Entities
{
    public class PlantingSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int CropId { get; set; }

        [ForeignKey("CropId")]
        public virtual Crop? Crop { get; set; } // <--- ADD THIS (Fixes Controller error)

        [Required]
        public int FieldId { get; set; }

        [ForeignKey("FieldId")]
        public virtual Field? Field { get; set; } // <--- ADD THIS (Fixes Controller error)

        public DateTime PlantingDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}