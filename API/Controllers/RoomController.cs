using API.Data;
using DomainModels.DB;
using DomainModels.DTO;
using DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly HotelContext _context;
    public RoomController(HotelContext context)
    {
        // Fill the DB Context through Dependency Injection
        _context = context;
    }



 

    [HttpGet] // Get all room
    
    public async Task<ActionResult<IEnumerable<Room>>> GetAllRooms()
    {
        // Currently the simplest CRUD operation
        var room = await _context.Rooms.ToListAsync();
        
        // <returns>Status OK with the list of rooms</returns>
        return Ok(room);
    }

   
    // Get specific ID of room
    
    [HttpGet("{id}")]// <param name="id">This is the id in the database of the room you want to select</param>
    public async Task<ActionResult<Room>> GetRoom(int id)
    {
        // Currently the simplest CRUD operation
        var room = await _context.Rooms.FindAsync(id);
        //Just in case null it returns not found
        if (room == null)
        {
            return NotFound();
        }

    // <returns>Status OK with room</returns>
        return Ok(room);
    }

    [HttpGet("types")]
    public async Task<ActionResult<List<Room>>> GetTypes()
    {
        try
        {
            // Get all exist rooms from the database
            var allRooms = await _context.Rooms.ToListAsync();

            // Create a new list of rooms
            List<Room> rooms = new List<Room>();

            foreach (var DBRoom in allRooms)
            {
                var searchedRoom = rooms.Find(r => r.Type == DBRoom.Type);
                if (searchedRoom != null)
                {
                    continue;
                }
                rooms.Add(DBRoom);
            }

            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpGet("GetTypes")]
    public async Task<List<string>> GetRoomTypes()
    {
        return await _context.Rooms.Select(r => r.Type).Distinct().ToListAsync();
    }



    // Add a new room

    /// <param name="room">Room object</param>
    /// <returns>Status OK with new room</returns>
    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(PostRoomDTO roomDTO) 
    {
        var room = new Room
        {
            RoomNumber = roomDTO.RoomNumber,
            Type = roomDTO.Type,
            Beds = roomDTO.Beds,
            Price = roomDTO.Price,
        };
        
        _context.Rooms.Add(room);
        _context.SaveChanges();
        return Ok(room);
    }

    /// Update Room
    
    /// <param name="room">Room object</param>
    /// <returns>Status OK with modified Room</returns>
    [HttpPut]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] Room room) 
    {
        // Currently the simplest CRUD operation
        _context.Rooms.Update(room);
        _context.SaveChanges();
        return Ok(room);
    }

    
    /// Delete Room by ID
    
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        // Currently the simplest CRUD operation
        var room = await _context.Rooms.FindAsync(id);
        _context.Rooms.Remove(room);
        _context.SaveChanges();
        return Ok(room);
    }


    
    
    [HttpGet("Search")]
    public async Task<IActionResult> Search([FromQuery] SearchRoomQueryDTO query )
    {
        // Create the start of a request to the DB
        var rooms = _context.Rooms
            // Make sure that the RoomType is included in the result
            .Include(r => r.Type)
            // Check if the room type of any of the rooms contains a tag that matches the search property of the query object
            .WhereIf(!string.IsNullOrWhiteSpace(query.Search), r => r.Tags.Any(tag => tag.ToLower().Contains(query.Search.ToLower())));

        // Check if SortBy is empty and if not then check if the entered value is a property on the Room object
        if (!string.IsNullOrWhiteSpace(query.SortBy) && typeof(Room).GetProperties().Any(p => p.Name.ToLower() == query.SortBy.ToLower()))
        {
            // Get the property name of the Room object that matches the SortBy property of the query object
            // (Just ensures capitalization and such is correct)
            string propName = typeof(Room).GetProperties().First(p => p.Name.ToLower() == query.SortBy.ToLower()).Name;

            // Check if the sort is ascending or descending
            if (query.IsSortAscending)
            {
                // Modify the request to return the rooms ordered by the property name in ascending order
                rooms = rooms.OrderBy(r => EF.Property<object>(r, propName));
            }
            else
            {
                // Modify the request to return the rooms ordered by the property name in descending order
                rooms = rooms.OrderByDescending(r => EF.Property<object>(r, propName));
            }
        }
        // Check if SortBy contains a value but it isn't a property of the Room object
        else if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            // Return a bad request with a message that the SortBy property isn't found in the Room object
            return BadRequest($"SortBy '{query.SortBy}' parameter not found in rooms object");
        }

        // Pagination
        rooms = rooms.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);

        // Return the paginated list of rooms
        return Ok(rooms.ToList());
        
    }

   
    /// Get the details of a room as a nice to digest DTO
    
   
    [HttpGet("RoomDetails/{id}")]
    public async Task<IActionResult> GetRoomDetails([FromRoute] int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            return NotFound();



        //var roomDetails = new GetRoomDetailsDTO
        //{

        //    Beds = room.Beds,
        //    Price = room.Price,
        //    Status = room.Status,
        //    Condition = room.Condition
        //};

        return Ok(
            );
    }


    [HttpGet("check-availability")]
    public async Task<IActionResult> IsRoomAvailable(string roomType, DateTime checkIn, DateTime checkOut)
    {
        // Retrieve all rooms of the specified type
        var existRooms = await _context.Rooms.Where(x => x.Type == roomType).ToListAsync();

        // If there are no rooms of that type, return an error
        if (existRooms.Count == 0)
        {
            return BadRequest("Room type doesn't exist.");
        }

        // Check each room for overlapping bookings
        foreach (var room in existRooms)
        {
            var isRoomAvailable = true;

            // Go through the BookedDates array to check for overlap
            foreach (var bookedDay in room.BookedDates)
            {
                // If at least one day overlaps with the booking range, the room is unavailable
                if (bookedDay >= checkIn && bookedDay < checkOut)
                {
                    isRoomAvailable = false;
                    break; // Exit the check for this room
                }
            }

            // If the room is available, return Ok() with the result true
            if (isRoomAvailable)
            {
                return Ok(true); // The room is available
            }
        }

        // If no rooms are available, return Ok(false)
        return Ok(false); // The rooms are unavailable
    [HttpGet("RoomDetails/{type}")]
    public async Task<IActionResult> GetRoomDetailsByType([FromRoute] string type)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Type == type);

        if (room == null)
            return NotFound();

        var roomDetails = new GetRoomDetailsDTO
        {
            Type = room.Type,
            Beds = room.Beds,
            Price = room.Price,
            Condition = room.Condition
        };

        return Ok(roomDetails);
    }
}



