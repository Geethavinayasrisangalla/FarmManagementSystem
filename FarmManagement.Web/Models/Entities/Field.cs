namespace FarmManagement.Web.Models.Entities
{
    public class Field
    {
        public int FieldId { get; set; }
        public string Name { get; set; }
        public float AreaSize { get; set; }
        public string SoilType { get; set; }
        public string LocationGPS { get; set; }
    }
}
