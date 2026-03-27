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

        // Optional: link to Field where resource was applied
        [ForeignKey(nameof(Field))]
        public int? FieldId { get; set; }

        [Required(ErrorMessage = "Quantity used is required")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity used must be greater than 0")]
        [Display(Name = "Quantity Used")]
        public double QuantityUsed { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date Applied")]
        public DateTime DateApplied { get; set; } = DateTime.Today;

        [StringLength(500)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        // Navigation properties
        public Resource? Resource { get; set; }
        public Field? Field { get; set; }
    }
}