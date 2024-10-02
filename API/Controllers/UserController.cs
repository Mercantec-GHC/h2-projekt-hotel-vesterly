using API.Data;
using API.Mappers;
using DomainModels.DB;
using DomainModels.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // This class is a controller class that handles the HTTP requests for the User entity
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
     
        // This is a constructor for the UserController class that takes in a HotelContext object to istantiate the database context
        private readonly HotelContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(HotelContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        /// <summary>
        /// Here we have an endpoint that gets a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) {
                return NotFound();
            }
            return Ok(user);
        }


        /// <summary>
        /// Here we have an endpoint that gets all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            if (users == null) {
                return NotFound();
            }
            return Ok(users);
        }

        [HttpGet("/User/Username/{username}")]
        public async Task<IActionResult> GetUserByUsername([FromRoute] string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// here we have a method that updates a user object by id.
        /// That is to be used by the customers or employee to update that specific customers account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [HttpPut("customer/{id}")]
        [Authorize]
        public IActionResult UpdateCustomer([FromRoute] int id, [FromBody] CreateUserDTO userDTO)
        {
            // first we find the customer by the id
            var user = _context.Users.Find(id);

            // if the customer is not found we return a not found status code
            if (user == null) {
                return NotFound();
            }
            // then we update the customer object with the new values from the CreateUserDTO object. 
            user.UserName = userDTO.Username;
            user.FirstName = userDTO.Firstname;
            user.LastName = userDTO.Lastname;
            user.PhoneNumber = userDTO.PhoneNumber;
            user.Email = userDTO.Email;
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok(user);
        }


        
        /// <summary>
        /// Here we have an endpoint that updates an employee by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="employeeDTO"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPut("employee/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] CreateEmployeeDTO employeeDTO)
        {
            // just like with the updating the customer, we also have to find the employee by the id first
                var user = await _context.Employees.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                // then we update the employee object with the new values from the CreateEmployeeDTO object.
                user.FirstName = employeeDTO.FirstName;
                user.LastName = employeeDTO.LastName;
                user.PhoneNumber = employeeDTO.phoneNumber;
                user.EmployeePhoneNumber = employeeDTO.EmployeePhoneNumber;
                user.UPN = employeeDTO.UPN;
                user.Email = employeeDTO.Email;
                _context.Employees.Update(user);
                _context.SaveChanges();
                return Ok(user);
     
        }

        /// <summary>
        /// This endpoint is used to assign the role of admin to a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("assignadmin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoleToUser([FromRoute] string id)
        {
            // first we find the user by the id
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // then we assign the role of admin to the user
            var assignToAdmin = await _userManager.AddToRoleAsync(user, "Admin");

            // if the assignment is successful we return an ok status code
            if (assignToAdmin.Succeeded)
            {
                return Ok();
            } else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// This endpoint is used to remove the role of admin from a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("removeadmin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRoleFromUser([FromRoute] string id)
        {
            // first we find the user by the id
            var user = await _context.Users.FindAsync(id);

            // if the user is not found we return a not found status code
            if (user == null)
            {
                return NotFound();
            }

            // then we remove the role of admin from the user
            var removeAdmin = await _userManager.RemoveFromRoleAsync(user, "Admin");

            // if the removal is successful we return an ok status code
            if (removeAdmin.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// here we have a method that deletes a user object by id. can be used for both customers and employees
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) {
                return NotFound();
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok();
        }
    }
}
