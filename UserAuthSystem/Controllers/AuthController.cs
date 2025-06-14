using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserAuthSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("/auth")]
        public async Task<IActionResult> UserRegister([FromBody] UserCreateDTO user)
        {
            try
            {
                var createdUser = await _userService.UserCreate(user);
                return CreatedAtAction(nameof(UserRegister), new { name = createdUser.Name }, createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("/login")]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginDTO user)
        {
            try
            {
                string token = await _userService.UserLogin(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
