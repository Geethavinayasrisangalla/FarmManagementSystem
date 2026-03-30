using FarmManagement.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.ViewModels;

public class InventoryViewModel
{
    public int ResourceId { get; set; }

    [Required(ErrorMessage = "Resource name is required.")]
    [StringLength(100)]
    [Display(Name = "Resource Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Type")]
    public ResourceType Type { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    [Display(Name = "Quantity")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    [Display(Name = "Unit (kg / L / units)")]
    public string Unit { get; set; } = string.Empty;
}