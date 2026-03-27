using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.Web.Models.Entities
{
    public class PlantingSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        [ForeignKey("Crop")]
        public int CropId { get; set; }

        [Required]
        [ForeignKey("Field")]
        public int FieldId { get; set; }

        [Required]
        public DateTime PlantingDate { get; set; }

        public DateTime EstimatedHarvestDate { get; set; }
        [Required]
        public string Status { get; set; } = "Planned";

        public Crop Crop { get; set; }
        public Field Field { get; set; }
    }
}

