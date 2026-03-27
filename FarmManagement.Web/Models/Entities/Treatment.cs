using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmManagement.Web.Models.Entities
{
    public class Treatment
    {
        [Key]
        public int TreatmentId { get; set; }

        public int IncidentId { get; set; }

        [ForeignKey("IncidentId")]
        public PestIncident PestIncident { get; set;}

        // [Required(ErrorMessage = "Please select a resource or chemical.")]
        public string ResourceId { get; set; }

        [Required]
        // [Display(Name="Treatment Date"]
        public DateTime TreatmentDate { get; set; }

        [Required]
        public string TreatmentType { get; set; }

        // For deduction
        // public decimal QuantityUsed {get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }
    }
}   
