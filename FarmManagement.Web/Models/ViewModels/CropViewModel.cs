using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.ViewModels
{
    public class CropViewModel
    {
        [Required(ErrorMessage = "Crop name is required")]
        [Display(Name = "Crop Name")]
        public string CommonName { get; set; } = string.Empty;

        [Display(Name = "Scientific Name")]
        public string? ScientificName { get; set; }

        public string? Category { get; set; }

        [Display(Name = "Season")]
        public string? RecommendedSeason { get; set; }
    }
}