using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeConnect.Application.DTOs;
using WeConnect.Application.Services;
using WeConnect.Domain.Interfaces;

namespace WeConnect.API.Controllers;

[ApiController]
[Route("api/v1/templates")]
[Produces("application/json")]
public class TemplatesController : ControllerBase
{
    private readonly TemplateService _templateService;
    private readonly ITemplateRepository _repo;

    public TemplatesController(TemplateService templateService, ITemplateRepository repo)
    {
        _templateService = templateService;
        _repo = repo;
    }

    // GET /api/v1/templates
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _templateService.GetAllAsync(_repo, ct);
        return Ok(new { success = true, data = items });
    }

    // GET /api/v1/templates/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var item = await _templateService.GetByIdAsync(_repo, id, ct);
        return item is null
            ? NotFound(new { success = false, message = "Template not found" })
            : Ok(new { success = true, data = item });
    }

    // GET /api/v1/templates/key/{key}
    [HttpGet("key/{key}")]
    public async Task<IActionResult> GetByKey(string key, CancellationToken ct)
    {
        var item = await _templateService.GetByKeyAsync(_repo, key, ct);
        return item is null
            ? NotFound(new { success = false, message = "Template not found" })
            : Ok(new { success = true, data = item });
    }

    // POST /api/v1/templates
    [HttpPost]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTemplateRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        var created = await _templateService.CreateAsync(_repo, request, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            new { success = true, data = created });
    }

    // PUT /api/v1/templates/{id}
    [HttpPut("{id:guid}")]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTemplateRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        var updated = await _templateService.UpdateAsync(_repo, id, request, ct);
        return updated is null
            ? NotFound(new { success = false, message = "Template not found" })
            : Ok(new { success = true, data = updated });
    }

    // DELETE /api/v1/templates/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _templateService.DeleteAsync(_repo, id, ct);
        return !deleted
            ? NotFound(new { success = false, message = "Template not found" })
            : Ok(new { success = true, message = "Template deleted" });
    }
}
