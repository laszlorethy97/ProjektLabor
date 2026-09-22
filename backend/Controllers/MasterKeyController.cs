using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasterKeyController : ControllerBase
{
    private readonly MasterKeyService _service;

    public MasterKeyController(MasterKeyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<MasterKey>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
}