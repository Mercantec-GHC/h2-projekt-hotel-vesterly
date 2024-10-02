using Microsoft.AspNetCore.Mvc;
using DomainModels.DTO;
using API.Data;
using DomainModels.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    /// <summary>
    /// This class is a controller class that handles the HTTP requests for the User entity 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TicketController : Controller
    {
        private readonly HotelContext _context;
        private readonly UserManager<User> _userManager;

        public TicketController(HotelContext context, UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        /// <summary>
        /// Fetch all tickets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _context.Tickets.ToListAsync();

            if (tickets == null)
            {
                return NotFound();
            }

            return Ok(tickets);
        }

        /// <summary>
        /// Fetch a ticket using the id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            //Just in case null, returns not found
            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        /// <summary>
        /// Add a new ticket
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return Ok(ticket);
        }

        /// <summary>
        /// Updating an already existing ticket, this can also be a part of the message/reply feature for now 
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            _context.SaveChanges();
            return Ok(ticket);
        }

        /// <summary>
        /// Removing/deleting an existing ticket, may want to do so in another way in the future, where it instead stores the ticket 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
            return Ok(ticket);
        }

        /// <summary>
        /// Fetch all messages for a specific ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        [HttpGet("{ticketId}/messages")]
        public async Task<IActionResult> GetTicketMessages([FromRoute] int ticketId)
        {
            var ticket = await _context.Tickets.Include(t => t.Messages).FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket.Messages);
        }

        /// <summary>
        /// Fetch a specific message for a specific ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpGet("{ticketId}/messages/{messageId}")]
        public async Task<IActionResult> GetTicketMessage([FromRoute] int ticketId, [FromRoute] int messageId)
        {
            var ticket = await _context.Tickets.Include(t => t.Messages).FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            var message = ticket.Messages.FirstOrDefault(m => m.Id == messageId);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        /// <summary>
        /// Add a new message to a specific ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="createTicketMessageDTO"></param>
        /// <returns></returns>
        [HttpPost("{ticketId}/messages")]
        [Authorize]
        public async Task<IActionResult> AddTicketMessage([FromRoute] int ticketId, [FromBody] CreateTicketMessageDTO createTicketMessageDTO)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(User.GetUsername());

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var message = new Message
            {
                TimeMessageSent = DateTime.Now,
                User = user,
                Ticket = ticket,
                MessageText = createTicketMessageDTO.MessageText
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }

        /// <summary>
        /// Update an existing message for a specific ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="messageId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPut("{ticketId}/messages/{messageId}")]
        [Authorize]
        public async Task<IActionResult> UpdateTicketMessage([FromRoute] int ticketId, [FromRoute] int messageId, [FromBody] Message message)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            var existingMessage = ticket.Messages.FirstOrDefault(m => m.Id == messageId);

            if (existingMessage == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(User.GetUsername());

            if (existingMessage.User.UserName != User.GetUsername() && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Unauthorized();
            }


            existingMessage.MessageText = message.MessageText;
            _context.Messages.Update(existingMessage);
            _context.SaveChanges();

            return Ok(existingMessage);
        }

        /// <summary>
        /// Delete an existing message for a specific ticket
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpDelete("{ticketId}/messages/{messageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTicketMessage([FromRoute] int ticketId, [FromRoute] int messageId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            var message = ticket.Messages.FirstOrDefault(m => m.Id == messageId);

            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            _context.SaveChanges();

            return Ok(message);
        }
    }
}
