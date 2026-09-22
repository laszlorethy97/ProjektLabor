using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly PermissionService _service;

    public PermissionController(PermissionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Permission>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
}