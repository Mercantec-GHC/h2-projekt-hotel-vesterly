using Microsoft.AspNetCore.Mvc;
using DomainModels.DTO;
using API.Data;
using DomainModels.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    
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

      
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return Ok(ticket);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            _context.SaveChanges();
            return Ok(ticket);
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
            return Ok(ticket);
        }

       
        [HttpGet("{ticketId}/messages")]
        public async Task<IActionResult> GetTicketMessages([FromRoute] int ticketId)
        {
            var messages = await _context.Messages.Include(m => m.User).Where(m => m.Ticket.Id == ticketId).ToListAsync();

            if (messages == null || messages.Count == 0)
            {
                return NotFound();
            }

            return Ok(messages);
        }

       
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

       
        [HttpPost("{ticketId}/messages")]
        //[Authorize]
        public async Task<IActionResult> AddTicketMessage([FromRoute] int ticketId, [FromBody] CreateTicketMessageDTO createTicketMessageDTO)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            User user;
            try
            {
                user = await _userManager.FindByNameAsync(User.GetUsername());

                if (user == null)
                {
                    user = await _userManager.FindByNameAsync("a");
                }
            }
            catch
            {
                user = await _userManager.FindByNameAsync("a");
            }

            var message = new Message
            {
                TimeMessageSent = DateTime.UtcNow,
                User = user,
                Ticket = ticket,
                MessageText = createTicketMessageDTO.MessageText
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }

       
        [HttpPut("{ticketId}/messages/{messageId}")]
        //[Authorize]
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

        
        [HttpDelete("{ticketId}/messages/{messageId}")]
        //[Authorize(Roles = "Admin")]
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
