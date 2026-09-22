using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRoleController : ControllerBase
{
    private readonly UserRoleService _service;

    public UserRoleController(UserRoleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserRole>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
}