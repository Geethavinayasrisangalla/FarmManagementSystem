using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.Web.Models.Entities
{
    public class Harvest
    {
        [Key]
        public int HarvestId { get; set; }

        [Required]
        [ForeignKey("PlantingSchedule")]
        public int ScheduleId { get; set; }

        [Required]
        [ForeignKey("Crop")]
        public int CropId { get; set; }

        [Required]
        [ForeignKey("Field")]
        public int FieldId { get; set; }

        [Required]
        public DateTime HarvestDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal HarvestedQuantity { get; set; }
    
        public string QualityGrade { get; set; }
        public PlantingSchedule PlantingSchedule { get; set; }
        public Crop Crop { get; set; }
        public Field Field { get; set; }
    }
}
