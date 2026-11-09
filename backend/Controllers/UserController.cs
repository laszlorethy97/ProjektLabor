using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using KeyManagement.Api.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service)
    {
        _service = service;
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO  loginUserDTO)
    {
        string? token = await _service.LoginUser(loginUserDTO);
        if (token == null) return BadRequest(new { message = "Invalid email or password." });
        return Ok(new { message = token });
    }

    [HttpPost("register")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Register(RegisterUserDTO registerUserDTO)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId))
        {
            return Unauthorized();
        }

        var result = await _service.RegisterUserAsync(registerUserDTO, adminId);

        if (result.Success)
        {
            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "User registered successfully."
            });
        }

        var errorResponse = new { message = result.Error };
        return result.ErrorCode switch
        {
            UserRegistrationError.EmailAlreadyRegistered => Conflict(errorResponse),
            UserRegistrationError.AdministratorNotFound => NotFound(errorResponse),
            _ => BadRequest(errorResponse)
        };
    }
}