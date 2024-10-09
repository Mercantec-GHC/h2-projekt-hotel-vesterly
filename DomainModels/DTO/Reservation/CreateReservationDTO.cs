using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.DTO
{
    public class CreateReservationDTO
    {
        [Required]
        public int RoomId { get; set; }
        public string GuestName { get; set; } = null!;
        public string GuestEmail { get; set; } = null!;
        public string? GuestPhoneNr { get; set; }
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
    }
}