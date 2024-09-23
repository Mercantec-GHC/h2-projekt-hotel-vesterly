using DomainModels.DB;
using DomainModels.DTO;
using DomainModels.DTO.Reservation;
using System.Net.Http;

namespace Blazor.Services;

public class DatabaseServices
{
    private readonly HttpClient _http;
    private readonly string _baseURL = "https://localhost:7207/";

    public DatabaseServices(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<GetRoomDetailsDTO>> GetRooms()
    {
        return await _http.GetFromJsonAsync<List<GetRoomDetailsDTO>>(_baseURL + "rooms");
    }

    public async Task<GetRoomDetailsDTO> GetRoom(int id)
    {
        return await _http.GetFromJsonAsync<GetRoomDetailsDTO>(_baseURL + "rooms/{id}");
    }

    public async Task<Room> AddRoom(Room room)
    {
        var response = await _http.PostAsJsonAsync(_baseURL + "rooms", room);
        return await response.Content.ReadFromJsonAsync<Room>();
    }

    public async Task<Room> UpdateRoom(Room room)
    {
        var response = await _http.PutAsJsonAsync(_baseURL + "rooms/{room.Id}", room);
        return await response.Content.ReadFromJsonAsync<Room>();
    }

    public async Task DeleteRoom(int id)
    {
        await _http.DeleteAsync(_baseURL + "rooms/{id}");
    }

    public async Task<List<Room>> GetAllRoomsTypes()
    {
        return await _http.GetFromJsonAsync<List<Room>>(_baseURL + "Room/types") ?? new();
    }

    public async Task <List<GetReservationsDTO>> GetAllReservations()
    {
        return await _http.GetFromJsonAsync<List<GetReservationsDTO>>(_baseURL + "Reservations") ?? new();
    }
}
