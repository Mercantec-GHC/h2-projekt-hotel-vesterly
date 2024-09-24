using API.Data;
using API.Extensions;
using DomainModels.DB;
using DomainModels.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Controller for reservation endpoints
    /// 
    /// Most likely wanna later add some more complex logic here to handle creation of reservations based off of user and room availability and such.
    /// </summary>
    /// 
    
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

        /// <summary>
        /// Get all reservations
        /// </summary>
        /// <returns>Status OK with the list of reservations</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Currently the simplest CRUD operation
            var reservations = await _context.Reservations.ToListAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// Get specific reservation by ID
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>Status OK with reservation</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // Currently the simplest CRUD operation
            var reservation = await _context.Reservations.FindAsync(id);
            return Ok(reservation);
        }

        /// <summary>
        /// Add new reservation
        /// </summary>
        /// <param name="reservation">Reservation object</param>
        /// <returns>Status CREATED</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody] CreateReservationDTO reservation)
        {
            // Check if the data fulfills the requirements of the DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by username
            var username = User.GetUsername();
            var appuser = await _userManager.FindByNameAsync(username);

            Room? room = await _context.Rooms.Include(r => r.Reservations).FirstAsync(r => r.Id == reservation.RoomId);
            User? customer = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (room == null || customer == null)
            {
                return BadRequest("Room ID could not be found.");
            }

            // Create time span for reservation
            TimeSpan span = reservation.CheckOut - reservation.CheckIn;

            // Check if the room is available
            if (room.Reservations.Any(r => ((r.CheckIn <= reservation.CheckOut && reservation.CheckIn < r.CheckOut) || (reservation.CheckIn <= r.CheckOut && r.CheckIn < reservation.CheckOut))))
            {
                return BadRequest("Room is already reserved for the selected dates.");
            }

            Reservation res = new Reservation
            {
                Rooms = new List<Room> { room },
                Customer = customer,
                Price = room.Price,
                CheckIn = reservation.CheckIn,
                CheckOut = reservation.CheckOut
            };

            _context.Reservations.Add(res);
            _context.SaveChanges();
            return Created();
        }

        /// <summary>
        /// Update reservation
        /// </summary>
        /// <param name="modifyReservation">Reservation object</param>
        /// <returns>Status OK with the modified reservation</returns>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ModifyReservationDTO modifyReservation)
        {
            // Check if the data fulfills the requirements of the DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user information
            var username = User.GetUsername();
            var appuser = await _userManager.FindByNameAsync(username);

            // Find reservation by ID
            var reservation = _context.Reservations.Find(modifyReservation.ReservationId);
            if (reservation == null) {
                return BadRequest("Reservation ID could not be found.");
            }

            // Check if user is admin role or user role
            if (!User.IsInRole("Admin"))
            { 
                // User can only update their own reservation
                if (reservation.Customer.Id != appuser.Id)
                {
                    return Unauthorized("You can only modify your own reservations!");
                }
            }

            // Modify reservation
            reservation.CheckIn = modifyReservation.CheckIn;
            reservation.CheckOut = modifyReservation.CheckOut;
            _context.Reservations.Update(reservation);

            // Save changes
            _context.SaveChanges();
            return Ok(modifyReservation);
        }

        /// <summary>
        /// Delete reservation by ID
        /// </summary>
        /// <param name="id">Integer ID of reservation</param>
        /// <returns>Status OK</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            // Get user information
            var username = User.GetUsername();
            var appuser = await _userManager.FindByNameAsync(username);
            
            // Find reservation by ID
            var reservation = await _context.Reservations.FindAsync(id);

            // Check if user is admin role or user role
            if (!User.IsInRole("Admin"))
            {
                // User can only delete their own reservation
                if (reservation.Customer.Id != appuser.Id)
                {
                    return Unauthorized("You can only delete your own reservations!");
                }
            }

            // Remove reservation and save changes
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
            return Ok();
        }
    }
}
