using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace FarmManagement.Web.Models.Entities
{
    public class PestIncident
    {
        [Key]
        public int IncidentId { get; set; }

        public int FieldId { get; set; }
        public int CropId { get; set; }
        public DateTime IncidentDate { get; set; } = DateTime.Now;
        public string Description { get; set; }

        // Not present in the pdf, but is added for better tracking
        public string Status { get; set; } = "Open"; // Open, In Progress, Resolved

        // Navigation property
        public List<Treatment> Treatments { get; set; } = new List<Treatment>();

    }
}
