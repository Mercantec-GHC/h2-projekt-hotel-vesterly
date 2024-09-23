using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.DTO.Reservation;

public class GetReservationsDTO
{
    public string GuestName { get; set; } = null!;
    public string GuestEmail { get; set; } = null!;
    public string? GuestPhoneNr { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Price{ get; set;}
    public string RoomType { get; set; } = null!;
    public string RoomNumber { get; set; } = null!;
    public List<DateTime> BookedDays { get; set; } = new List<DateTime>();
}
