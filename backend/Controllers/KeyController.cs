using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KeyController : ControllerBase
{
    private readonly KeyService _service;

    public KeyController(KeyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Key>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
}