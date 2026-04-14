using Fanscore.Application.DTOs.Tournament;
using Fanscore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class TournamentController : ControllerBase
{
    private readonly ITournamentService _service;

    public TournamentController(ITournamentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetByIdAsync(id);
        if (data == null) return NotFound();

        return Ok(data);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(TournamentCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var created = await _service.CreateAsync(userId, dto);
        return Ok(created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TournamentUpdateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _service.UpdateAsync(id, userId, dto);
        if (!result) return NotFound();

        return Ok(new { message = "Güncellendi" });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _service.DeleteAsync(id, userId);
        if (!result) return NotFound();

        return Ok(new { message = "Silindi" });
    }
}