namespace FarmManagement.Web.Models.Entities
{
    public class Harvest
    {
        public int HarvestId { get; set; }

        public int CropId { get; set; }     
        public int FieldId { get; set; }   

        public DateTime HarvestDate { get; set; }
        public decimal HarvestedQuantity { get; set; } 
    }
}
