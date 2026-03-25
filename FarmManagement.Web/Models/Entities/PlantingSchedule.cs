namespace FarmManagement.Web.Models.Entities
{
    public class PlantingSchedule
    {
        public int ScheduleId { get; set; } 

        public int CropId { get; set; }    
        public int FieldId { get; set; }   

        public DateTime PlantingDate { get; set; }
        public string Status { get; set; } 
    }
}
