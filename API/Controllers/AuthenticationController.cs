
using API.Services;
using DomainModels.DB;
using DomainModels.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    
    /// Authorization controller
    /// Handles anything to do with login and registration of users
    
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        public AuthenticationController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        /// Login endpoint
        /// <param name="loginDTO">Login DTO consisting of username and password</param>
        /// <returns>UserTokenDTO which consists of username, email, and JWT token used in future requests to locked services</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by username
            List<User> users = _userManager.Users.ToList();

            var user = users.First(e => e.UserName == loginDTO.Username);

            // If the user is not found, return unauthorized
            if (user == null)
            {
                return Unauthorized("Username is invalid");
            }

            // Check if the password is correct
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            // If the password is incorrect, return unauthorized
            if (!result.Succeeded)
            {
                return Unauthorized("Username or password is invalid");
            }

            // Return the user token DTO and create a token for the user to put in the response
            return Ok(
                new UserTokenDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                });

        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="createUsersDTO">CreateUserDTO consisting of usernname, firstname, lastname, phonenumber, email, and password</param>
        /// <returns>UserTokenDTO which consists of username, email, and jwt token</returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO createUsersDTO)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Create a new user object from the DTO
                var user = new User
                {
                    UserName = createUsersDTO.Username,
                    FirstName = createUsersDTO.Firstname,
                    LastName = createUsersDTO.Lastname,
                    PhoneNumber = createUsersDTO.PhoneNumber,
                    Email = createUsersDTO.Email
                };

                // Create the user in the database
                var createUser = await _userManager.CreateAsync(user, createUsersDTO.Password);

                // If the user is created, add the user to the role of User, and return the user token DTO
                if (createUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new UserTokenDTO
                            {
                                UserName = user.UserName,
                                Email = user.Email,
                                Token = _tokenService.CreateToken(user)
                            }
                            );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
