using API.Data;
using API.Extensions;
using DomainModels.DB;
using DomainModels.DTO;
using DomainModels.DTO.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace API.Controllers
{
    
    /// Controller for reservation endpoints
    /// Most likely wanna later add some more complex logic here to handle creation of reservations based off of user and room availability and such.
    
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : Controller
    {

        private readonly HotelContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationsController(HotelContext context, UserManager<User> userManager)
        {
            // Fill the DB Context through Dependency Injection
            _context = context;
            _userManager = userManager;
        }

        
        /// Get all reservations
       
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            
            var reservations = await _context.Reservations.Include(x => x.Room).Include(x => x.Extras).ToListAsync();
            var reservationsDTO = reservations.ConvertAll(x => new GetReservationsDTO
            {
                ReservationId = x.Id,
                RoomType = x.Room.Type,
                RoomNumber = x.Room.RoomNumber,
                GuestName = x.GuestName,
                GuestEmail = x.GuestEmail,
                GuestPhoneNr = x.GuestPhoneNr,
                Price = x.Room.Price,
                CheckIn = x.CheckIn,
                CheckOut = x.CheckOut
            });
          
               
         

            return Ok(reservationsDTO);

        }

        
        /// Get specific reservation by ID
        
        [HttpGet("{id}")]
        public IActionResult GetReservationById(int id)
        {
            var reservation =  _context.Reservations.Include(x => x.Room).Where(x => x.Id == id).First();
            if (reservation == null)
            {
                return BadRequest("Reservation ID could not be found.");
            }
            return Ok(reservation);
        }

      
        /// Add new reservation
       
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Post(CreateReservationDTO reservation)
        {
            // Check if the data fulfills the requirements of the DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by username
            //var username = User.GetUsername();
            //var appuser = await _userManager.FindByNameAsync(username);

            Room? room = await _context.Rooms.FirstAsync(r => r.Id == reservation.RoomId);
            //User? customer = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (room == null)
            {
                return BadRequest("Room ID could not be found.");
            }

            // Create time span for reservation
            TimeSpan span = reservation.CheckOut - reservation.CheckIn;

            // Check if the room is available

            room.BookedDates.Sort();
            var checkIn = room.BookedDates.FirstOrDefault();
            var checkOut = room.BookedDates.LastOrDefault();

            if (room.BookedDates.Any(r => ((checkIn <= reservation.CheckOut && reservation.CheckIn < checkOut) || (reservation.CheckIn <= checkOut && checkIn < reservation.CheckOut))))
            {
                return BadRequest("Room is already reserved for the selected dates.");
            }

            Room roomId = _context.Rooms.First(x => x.Id == reservation.RoomId) ?? new();


            Reservation res = new Reservation
            {
                Room = roomId,
                GuestName = reservation.GuestName,
                GuestPhoneNr = reservation.GuestPhoneNr,
                GuestEmail = reservation.GuestEmail,
                Price = room.Price,
                CheckIn = reservation.CheckIn,
                CheckOut = reservation.CheckOut
            };

            _context.Reservations.Add(res);

            room.BookedDates.AddRange(Enumerable
                .Range(0, (int)(reservation.CheckOut - reservation.CheckIn).TotalDays)
                .Select(i => DateTime.SpecifyKind(reservation.CheckIn, DateTimeKind.Utc)
                .AddDays(i)));

            _context.SaveChanges();
            return Created();
        }

       
        /// Update reservation
        
        [HttpPut("update")]
        //[Authorize]
        public async Task<IActionResult> Update(ModifyReservationDTO modifyReservation)
        {
            // Check if the data fulfills the requirements of the DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user information
           // var username = User.GetUsername();
            //var appuser = await _userManager.FindByNameAsync(username);

            // Find reservation by ID
            var reservation = _context.Reservations.Include(x => x.Room).Where(x => x.Id == modifyReservation.ReservationId).First();
            if (reservation == null) {
                return BadRequest("Reservation ID could not be found.");
            }

            // Check if user is admin role or user role
            //if (!User.IsInRole("Admin"))
            //{ 
            //    // User can only update their own reservation
            //    if (reservation.Customer.Id != appuser.Id)
            //    {
            //        return Unauthorized("You can only modify your own reservations!");
            //    }
            //}

            var existRooms = _context.Rooms.Where(x => x.Type == modifyReservation.RoomType).ToList();
            if (existRooms.Count == 0)
            {
                return BadRequest("Room doesn't available.");
            }

            Room availibleRoom = null!;

            foreach (var room in existRooms) { 
                var checkIn = room.BookedDates.FirstOrDefault();
                var checkOut = room.BookedDates.LastOrDefault();
           
                
                var isRoomAvailable = checkIn >= modifyReservation.CheckOut || checkOut <= modifyReservation.CheckIn;
                
                if (isRoomAvailable) {
                    availibleRoom = room;
                    break;
                }
            }

            if (availibleRoom == null)
            {
                return BadRequest("Room is already reserved for the selected dates.");
            }
            // Modify reservation
            reservation.GuestName = modifyReservation.GuestName;
            reservation.GuestPhoneNr = modifyReservation.GuestPhoneNr;
            reservation.GuestEmail = modifyReservation.GuestEmail;
            reservation.Room.Type = modifyReservation.RoomType;
            reservation.CheckIn = modifyReservation.CheckIn;
            reservation.CheckOut = modifyReservation.CheckOut;

            _context.Entry(reservation).State = EntityState.Modified;

            // Save changes
            _context.SaveChanges();
            return Ok(modifyReservation);
        }

       
        /// Delete reservation by ID
        
        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            // Get user information
           // var username = User.GetUsername();
            //var appuser = await _userManager.FindByNameAsync(username);
            
            // Find reservation by ID
            var reservation = await _context.Reservations.FindAsync(id);

            // Check if user is admin role or user role
            //if (!User.IsInRole("Admin"))
            //{
            //    // User can only delete their own reservation
            //    if (reservation.Customer.Id != appuser.Id)
            //    {
            //        return Unauthorized("You can only delete your own reservations!");
            //    }
            //}

            // Remove reservation and save changes
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
            return Ok();
        }
    }
}
