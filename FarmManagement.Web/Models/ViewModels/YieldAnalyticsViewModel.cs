namespace FarmManagement.Web.Models.ViewModels
{
    public class YieldAnalyticsViewModel
    {
        public string FieldName { get; set; }

        public double AreaSize { get; set; }

        public double TotalHarvested { get; set; }

        public double AverageYield { get; set; }

        public double CalculatedEfficiency { get; set; }
    }
}
