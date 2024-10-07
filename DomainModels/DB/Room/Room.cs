using System.ComponentModel.DataAnnotations;

namespace DomainModels.DB
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public int RoomNumber { get; set; }
        public string Beds { get; set; } = "";
        public int Price { get; set; }
        public RoomStatus Status { get; set; }
        public string Condition { get; set; } = "";
        public string Description { get; set; } = "";
       
        public virtual List<string> Tags { get; set; } = new List<string>();
    
        public List <DateTime> BookedDates { get; set; } = new List<DateTime>();
    }
       public enum RoomStatus
        {
            available,
            underMaintenece,
            needsCleaning
        }
}