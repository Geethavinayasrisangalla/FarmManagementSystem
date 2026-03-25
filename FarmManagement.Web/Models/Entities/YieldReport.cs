namespace FarmManagementSystem.Models.Entities
{
    public class YieldReport
    {
        public int YieldReportId { get; set; }

        public int FieldId { get; set; }

        public double CalculatedEfficiency { get; set; }

        public double TotalCostIncurred { get; set; }
    }
}
