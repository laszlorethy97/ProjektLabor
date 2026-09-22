using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildingController : ControllerBase
{
    private readonly BuildingService _service;

    public BuildingController(BuildingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Building>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
}