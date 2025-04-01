using identity.DTO.Account;
using identity.Extensions;
using identity.interfaces;
using Identity.DTO.Account;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace identity.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;


        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }


        [HttpPost("api/login")]
        [SwaggerOperation(
            Summary = "Login an existing user",
            Description = "Logs in a user by validating their username and password. On success, a JWT token is generated and returned."
        )]
        [SwaggerResponse(200, "User logged in successfully and JWT token generated.", typeof(NewUserDTO))]
        [SwaggerResponse(401, "Unauthorized - Invalid username or password.")]
        [SwaggerResponse(500, "Internal server error occurred during login.")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDTO.UserName);
                if (user == null)
                {
                    return Unauthorized("Invalid username");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Invalid password");
                }
                //
                return Ok(_signInManager.GenLoginToken(user, _tokenService)); //GenLoginToken(user) is an extension method in SignInExtensions.cs that returns a NewUserDTO object with the user's username, email, and a JWT token.
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [AllowAnonymous]
        [HttpPost("api/register")]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "Creates a new user account with the provided registration details. A role 'User' is assigned to the new user after successful creation."
        )]
        [SwaggerResponse(200, "User registered successfully and login token generated.")]
        [SwaggerResponse(400, "Bad request. Model state is invalid.")]
        [SwaggerResponse(500, "Internal server error occurred during user registration or role assignment.")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(_signInManager.GenLoginToken(appUser, _tokenService));
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}