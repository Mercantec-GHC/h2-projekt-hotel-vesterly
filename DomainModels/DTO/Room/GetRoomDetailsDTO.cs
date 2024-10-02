using DomainModels.DB;

namespace DomainModels.DTO;

public class GetRoomDetailsDTO
{
    public string Type { get; set; }
    public int Rooms { get; set; }
    public int RoomNumber { get; set; }
    public string? Beds { get; set; }
    public int Price { get; set; }
    public string Status { get; set; }
    public string? Condition { get; set; }
}


