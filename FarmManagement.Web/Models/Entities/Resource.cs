using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.Entities
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        [Display(Name = "Resource Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Type is required")]
        [StringLength(50)]
        [Display(Name = "Type")]
        public string Type { get; set; } = null!; // Seed / Fertilizer / Pesticide / Water

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(20)]
        [Display(Name = "Unit")]
        public string Unit { get; set; } = null!; // kg / Liters / bags

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        [Display(Name = "Quantity Available")]
        public double QuantityAvailable { get; set; }

        // Navigation
        public ICollection<ResourceUsage>? Usages { get; set; }
    }
}