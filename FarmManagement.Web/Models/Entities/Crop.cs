using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.Entities
{
    public class Crop
    {
        [Key]
        public int CropId { get; set; }
        [Required]
        public string CommonName { get; set; } = string.Empty;
        public string? ScientificName { get; set; }
        public string? Category { get; set; }
        public string? RecommendedSeason { get; set; }
    }
}