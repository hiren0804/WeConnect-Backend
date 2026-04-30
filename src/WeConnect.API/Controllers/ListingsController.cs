using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeConnect.Application.DTOs;
using WeConnect.Application.Services;
using WeConnect.Domain.Interfaces;
using WeConnect.Infrastructure.Persistence;
using WeConnect.Infrastructure.Repositories;

namespace WeConnect.API.Controllers;

[ApiController]
[Route("api/v1/listings")]
[Produces("application/json")]
public class ListingsController : ControllerBase
{
    private readonly ListingService _listingService;

    public ListingsController(ListingService listingService)
    {
        _listingService = listingService;
    }

    private Guid TenantId => (Guid)HttpContext.Items["TenantId"]!;
    private TenantDbContext TenantDb => (TenantDbContext)HttpContext.Items["TenantDb"]!;

    private IListingRepository GetRepo() => new ListingRepository(TenantDb);

    // GET /api/v1/listings
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? type,
        CancellationToken ct)
    {
        var repo = GetRepo();
        var items = type is not null
            ? await _listingService.GetByTypeAsync(repo, TenantId, type, ct)
            : await _listingService.GetAllAsync(repo, TenantId, ct);

        return Ok(new { success = true, data = items });
    }

    // GET /api/v1/listings/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var repo = GetRepo();
        var item = await _listingService.GetByIdAsync(repo, id, TenantId, ct);
        return item is null
            ? NotFound(new { success = false, message = "Listing not found" })
            : Ok(new { success = true, data = item });
    }

    // POST /api/v1/listings
    [HttpPost]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Create(
        [FromBody] CreateListingRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        var repo = GetRepo();
        var created = await _listingService.CreateAsync(repo, TenantId, request, ct);
        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            new { success = true, data = created });
    }

    // PUT /api/v1/listings/{id}
    [HttpPut("{id:guid}")]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateListingRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, errors = ModelState });

        var repo = GetRepo();
        var updated = await _listingService.UpdateAsync(repo, id, TenantId, request, ct);
        return updated is null
            ? NotFound(new { success = false, message = "Listing not found" })
            : Ok(new { success = true, data = updated });
    }

    // DELETE /api/v1/listings/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(AuthenticationSchemes = "HardcodedToken")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var repo = GetRepo();
        var deleted = await _listingService.DeleteAsync(repo, id, TenantId, ct);
        return !deleted
            ? NotFound(new { success = false, message = "Listing not found" })
            : Ok(new { success = true, message = "Listing deleted" });
    }
}
