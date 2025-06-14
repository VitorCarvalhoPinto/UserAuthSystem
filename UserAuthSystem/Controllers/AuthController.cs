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
            await _userService.UserCreate(user);
            return CreatedAtAction(nameof(UserRegister), new { name = user.Name }, user);
        }

    }
}
