using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.Web.Models.Entities
{
    public class Harvest
    {
        [Key]
        public int HarvestId { get; set; }

        public int FieldId { get; set; }

        [ForeignKey("FieldId")]
        public virtual Field? Field { get; set; }

        public double ActualYieldWeight { get; set; }
        public DateTime HarvestDate { get; set; }
        public string? QualityGrade { get; set; }
    }
}