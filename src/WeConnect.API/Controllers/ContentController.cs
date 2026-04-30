namespace WeConnect.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeConnect.Application.Services;
using WeConnect.Domain.Interfaces;
using WeConnect.Infrastructure.Persistence;
using WeConnect.Infrastructure.Repositories;

[ApiController]
[Route("api/content")]
public class ContentController(ContentService contentService) : ControllerBase
{
    // GET /api/content?type=blog
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string type,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(type))
            return BadRequest(new { error = "Query param 'type' is required. e.g. ?type=blog" });

        var tenantId = (Guid)HttpContext.Items["TenantId"]!;
        var tenantDb = (TenantDbContext)HttpContext.Items["TenantDb"]!;

        // Create repository with the per-request tenant DB context
        IContentRepository repo = new ContentRepository(tenantDb);
        var items = await contentService.GetItemsAsync(repo, tenantId, type, ct);
        return Ok(items);
    }

    // GET /api/content/{id}?type=blog
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        Guid id,
        [FromQuery] string type,
        CancellationToken ct)
    {
        var tenantId = (Guid)HttpContext.Items["TenantId"]!;
        var tenantDb = (TenantDbContext)HttpContext.Items["TenantDb"]!;

        // Create repository with the per-request tenant DB context
        IContentRepository repo = new ContentRepository(tenantDb);
        var item = await contentService.GetItemAsync(repo, id, tenantId, ct);
        return item is null ? NotFound() : Ok(item);
    }
}

