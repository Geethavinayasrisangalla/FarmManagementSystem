using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.Web.Models.Entities
{
    public class ResourceUsage
    {
        [Key]
        public int UsageId { get; set; }

        [Required]
        [ForeignKey(nameof(Resource))]
        public int ResourceId { get; set; }

        // optional: link to Field if you have Field entity
        public int? FieldId { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity used must be positive")]
        public double QuantityUsed { get; set; }

        [Required]
        public DateTime DateApplied { get; set; } = DateTime.UtcNow;

        public string? Remarks { get; set; }

        public Resource? Resource { get; set; }
    }
}
