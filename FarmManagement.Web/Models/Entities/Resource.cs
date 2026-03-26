using FarmManagement.Web.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.Web.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = null!; // Seed/Fertilizer/Pesticide

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = null!; // kg/Liters

        [Range(0, int.MaxValue)]
        public int QuantityAvailable { get; set; }

        // navigation
        public ICollection<ResourceUsage>? Usages { get; set; }
    }
}
