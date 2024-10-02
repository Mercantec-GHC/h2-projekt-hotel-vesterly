using DomainModels.DB;
using System.ComponentModel.DataAnnotations;

namespace DomainModels.DTO
{
    public class ModifyReservationDTO
    {
        [Required]
        public int ReservationId { get; set; }

        public string GuestName { get; set; } = null!;
        public string GuestEmail { get; set; } = null!;
        public string? GuestPhoneNr { get; set; }
        public string RoomType { get; set; } = null!;
        public int RoomPrice { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
