using KeyManagement.Api.Models.Entities;
using KeyManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasterKeyTransactionController : ControllerBase
{
    private readonly MasterKeyTransactionService _service;

    public MasterKeyTransactionController(MasterKeyTransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<MasterKeyTransaction>>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
}