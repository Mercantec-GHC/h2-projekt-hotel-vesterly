using Blazor.Auth;
using DomainModels.DB;
using DomainModels.DTO;
using DomainModels.DTO.Reservation;


namespace Blazor.Services;

public class DatabaseServices
{
    private readonly HttpClient _http;
    private readonly string _baseURL = "https://localhost:7207/";
    private readonly AuthService _authService;


    public DatabaseServices(HttpClient http, AuthService authService)
    {
        _http = http;
        _authService = authService;
    }


    //----- Room -------//
    public async Task<List<Room>> GetRooms()
    {

        return await _authService.GetFrom<List<Room>>(_baseURL + "Room/GetAll") ?? new();
    }

    public async Task<GetRoomDetailsDTO> GetRoom(int id)
    {
        return await _http.GetFromJsonAsync<GetRoomDetailsDTO>(_baseURL + "Room/{id}") ?? new();
    }

    public async Task<Room> AddRoom(Room room)
    {
        var response = await _http.PostAsJsonAsync(_baseURL + "Room", room);
        return await response.Content.ReadFromJsonAsync<Room>();
    }

    public async Task<Room> UpdateRoom(Room room)
    {
        var response = await _http.PutAsJsonAsync(_baseURL + "Room/{id}", room);
        return await response.Content.ReadFromJsonAsync<Room>();
    }

    public async Task DeleteRoom(int id)
    {
        await _http.DeleteAsync(_baseURL + "Room/{id}");
    }

    public async Task<List<Room>> GetAllRoomsTypes()
    {
        return await _http.GetFromJsonAsync<List<Room>>(_baseURL + "Room/types") ?? new();
    }

    //----- Reservation -------//

    public async Task <List<GetReservationsDTO>> GetAllReservations()
    {
        return await _http.GetFromJsonAsync<List<GetReservationsDTO>>(_baseURL + "Reservations") ?? new();
    }

    public Task<Reservation> GetReservationByIdAsync(int id)
    {
        return _http.GetFromJsonAsync<Reservation>(_baseURL +"Reservations/{id}");
    }
}
