using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KeyTransactionController : ControllerBase
{
    private readonly KeyTransactionService _service;

    public KeyTransactionController(KeyTransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<KeyTransaction>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
}