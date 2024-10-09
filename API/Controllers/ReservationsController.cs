﻿using API.Data;
using DomainModels.DB;
using DomainModels.DTO;
using DomainModels.DTO.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


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

    [HttpGet("byUsername/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var reservations = await _context.Reservations.Where(r => r.Customer.UserName == username).ToListAsync();
        return Ok(reservations);
    }




    [HttpGet("{id}")]
    public IActionResult GetReservationById(int id)
    {
        var reservation = _context.Reservations.Include(x => x.Room).Where(x => x.Id == id).First();
        if (reservation == null)
        {
            return BadRequest("Reservation ID could not be found.");
        }
        return Ok(reservation);
    }




    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> Post([FromBody] Reservation reservation)
    {
        // Check if the data fulfills the requirements of the DTO
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Find user by username
        User customer;
        try
        {
            customer = await _userManager.FindByNameAsync(User.GetUsername());

            if (customer == null)
            {
                customer = await _userManager.FindByNameAsync("a");
            }
        }
        catch
        {
            customer = await _userManager.FindByNameAsync("a");
        }

        Room? room = await _context.Rooms.FirstAsync(r => r.Id == reservation.Room.Id);

        if (room == null || customer == null)
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

        Room roomId = _context.Rooms.First(x => x.Id == reservation.Room.Id) ?? new();


        Reservation res = new Reservation
        {
            
            GuestName = reservation.GuestName,
            GuestPhoneNr = reservation.GuestPhoneNr,
            GuestEmail = reservation.GuestEmail,
            Room = room,
            Customer = customer,
            CheckIn = reservation.CheckIn,
            CheckOut = reservation.CheckOut
        };

        _context.Reservations.Add(res);

        _context.SaveChanges();
        await UpdateBookedDates(room.Id);
        return Created();
    }


   

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update(ModifyReservationDTO modifyReservation)
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
        var reservation = _context.Reservations
        .Include(x => x.Room)
        .Where(x => x.Id == modifyReservation.ReservationId)
        .First();

        var prevRoom = reservation.Room;

        if (reservation == null)
        {
            return BadRequest("Reservation ID could not be found.");
        }

        Check if user is admin role or user role
        if (!User.IsInRole("Admin"))
            {
                // User can only update their own reservation
                if (reservation.Customer.Id != appuser.Id)
                {
                    return Unauthorized("You can only modify your own reservations!");
                }
            }

        var existRooms = _context.Rooms
            .Where(x => x.Type == modifyReservation.RoomType)
            .ToList();

        if (existRooms.Count == 0)
        {
            return BadRequest("Room doesn't available.");
        }

        Room availibleRoom = null!;

        foreach (var room in existRooms)
        {
            var isRoomAvailable = !room.BookedDates
                .Any(d => d >= modifyReservation.CheckIn && d < modifyReservation.CheckOut);

            if (isRoomAvailable)
            {
                availibleRoom = room;
                break;
            }
        }

        if (availibleRoom == null)
        {
            return BadRequest("No rooms available for the selected dates.");
        }


        // Modify reservation
        reservation.Room = availibleRoom;
        reservation.GuestName = modifyReservation.GuestName;
        reservation.GuestPhoneNr = modifyReservation.GuestPhoneNr;
        reservation.GuestEmail = modifyReservation.GuestEmail;
        reservation.CheckIn = modifyReservation.CheckIn;
        reservation.CheckOut = modifyReservation.CheckOut;



        _context.Entry(reservation).State = EntityState.Modified;
        _context.Entry(availibleRoom).State = EntityState.Modified;

        // Save changes
        _context.SaveChanges();
        await UpdateBookedDates(prevRoom.Id);
        await UpdateBookedDates(availibleRoom.Id);
        return Ok(modifyReservation);
    }



    [HttpDelete("{id}")]
    //[Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        // Get user information
        var username = User.GetUsername();
        var appuser = await _userManager.FindByNameAsync(username);

        // Find reservation by ID
        var reservation = _context.Reservations
            .Include(b => b.Room)
            .Where(b => b.Id == id)
            .FirstOrDefault();

        if (reservation == null)
        {
            return BadRequest("Reservation ID could not be found.");
        }

        // Check if user is admin role or user role
        if (!User.IsInRole("Admin"))
        {
            // User can only delete their own reservation
            if (reservation.Customer.Id != appuser.Id)
            {
                return Unauthorized("You can only delete your own reservations!");
            }
        }

        var room = reservation.Room;

        if (room == null)
        {
            return NotFound("Room associated with reservation not found.");
        }


        // Remove reservation and save changes
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        await UpdateBookedDates(room.Id);
        return Ok();
    }


    private async Task UpdateBookedDates(int roomId)
    {
        Room room = _context.Rooms.First(x => x.Id == roomId);

        var res = _context.Reservations
            .Where(r => r.Room.Id == room.Id)
            .ToList();

        room.BookedDates.Clear();

        foreach (var reservation in res)
        {

            int totalDays = (int)(reservation.CheckOut - reservation.CheckIn).TotalDays;


            var bookingDates = Enumerable.Range(0, totalDays)
                .Select(i => reservation.CheckIn.AddDays(i))
                .ToList();


            room.BookedDates.AddRange(bookingDates);


        }

        await _context.SaveChangesAsync();
    }

}