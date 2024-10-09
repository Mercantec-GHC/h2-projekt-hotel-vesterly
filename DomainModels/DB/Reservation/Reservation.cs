using System.ComponentModel.DataAnnotations;

namespace DomainModels.DB
{
    public class Reservation
    {
        [Key] 
        public int Id { get; set; }
        public string GuestName { get; set; } = null!;
        public string GuestEmail { get; set; } = null!;
        public string? GuestPhoneNr { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public User Customer { get; set; }
        public Room Room { get; set; }
        public List<Extra> Extras { get; set; } = new();
    }
}
