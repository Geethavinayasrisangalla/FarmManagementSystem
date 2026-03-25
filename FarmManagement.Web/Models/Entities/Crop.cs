namespace FarmManagement.Web.Models.Entities
{
    public class Crop
    {
        public int CropId { get; set; }
        public string CommonName { get; set; }
        public string ScientificName { get; set; }
        public string CategoName { get; set; }
        public string RecommendedSeason {  get; set; }
    }
}
