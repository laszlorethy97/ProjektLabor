using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;
using KeyManagement.Api.DTO;

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

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO  loginUserDTO)
    {
        string? token = await _service.LoginUser(loginUserDTO);
        if (token == null) return BadRequest(new { message = "Invalid email or password." });
        return Ok(new { message = token });
    }
}