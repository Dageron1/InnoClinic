using InnoClinic.OfficesService.DTOs;
using InnoClinic.OfficesService.Entities;
using InnoClinic.OfficesService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.OfficesService.Controllers;

[Route("api/offices")]
[ApiController]
public class OfficesController : ControllerBase
{
    private readonly IOfficeService _officeService;

    public OfficesController(IOfficeService officeService)
    {
        _officeService = officeService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Office>>> Get()
    {
        var offices = await _officeService.GetAllOfficesAsync();

        return Ok(offices);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Office>> Get(string id)
    {
        var office = await _officeService.GetOfficeByIdAsync(id);

        return Ok(office);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(OfficeDto office)
    {
        await _officeService.CreateOfficeAsync(office);

        return Created();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(string id, OfficeDto office)
    {
        await _officeService.UpdateOfficeAsync(id, office);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(string id)
    {
        await _officeService.DeleteOfficeAsync(id);
        return NoContent();
    }
}
