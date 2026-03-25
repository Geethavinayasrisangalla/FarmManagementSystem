using System.ComponentModel.DataAnnotations;

namespace FarmManagement.Web.Models.Entities
{
    public class Field
    {
        [Key]
        public int FieldId { get; set; }
        [Required]
        public string FieldName { get; set; } = string.Empty;
        public double AreaSize { get; set; }
        public string? SoilType { get; set; }
        public string? LocationGps { get; set; }
    }
}