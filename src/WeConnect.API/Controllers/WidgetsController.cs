using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeConnect.Application.DTOs;
using WeConnect.Application.Services;
using WeConnect.Domain.Interfaces;

namespace WeConnect.API.Controllers;

[ApiController]
[Route("api/v1/widgets")]
[Produces("application/json")]
public class WidgetsController : ControllerBase
{
    private readonly WidgetService _widgetService;
    private readonly IWidgetRepository _repo;

    public WidgetsController(WidgetService widgetService, IWidgetRepository repo)
    {
        _widgetService = widgetService;
        _repo = repo;
    }

    // GET /api/v1/widgets
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _widgetService.GetAllAsync(_repo, ct);
        return Ok(new { success = true, data = items });
    }

    // GET /api/v1/widgets/page/{pageId}
    [HttpGet("page/{pageId:guid}")]
    public async Task<IActionResult> GetByPageId(Guid pageId, CancellationToken ct)
    {
        var items = await _widgetService.GetByPageIdAsync(_repo, pageId, ct);
        return Ok(new { success = true, data = items });
    }

    // GET /api/v1/widgets/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var item = await _widgetService.GetByIdAsync(_repo, id, ct);
        return item is null
            ? NotFound(new { success = false, message = "Widget not found" })
            : Ok(new { success = true, data = item });
    }

    // POST /api/v1/widgets
    [HttpPost]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Create(
        [FromBody] CreateWidgetRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        var created = await _widgetService.CreateAsync(_repo, request, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            new { success = true, data = created });
    }

    // PUT /api/v1/widgets/{id}
    [HttpPut("{id:guid}")]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateWidgetRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        var updated = await _widgetService.UpdateAsync(_repo, id, request, ct);
        return updated is null
            ? NotFound(new { success = false, message = "Widget not found" })
            : Ok(new { success = true, data = updated });
    }

    // DELETE /api/v1/widgets/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _widgetService.DeleteAsync(_repo, id, ct);
        return !deleted
            ? NotFound(new { success = false, message = "Widget not found" })
            : Ok(new { success = true, message = "Widget deleted" });
    }
}
