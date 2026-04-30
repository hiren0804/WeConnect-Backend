namespace WeConnect.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeConnect.Application.Common.Interfaces;
using WeConnect.Infrastructure.Services;

[ApiController]
[Route("api/admin")]
public class AdminController(ProvisioningService provisioning, ITenantService tenantService) : ControllerBase
{
    public record CreateTenantRequest(
        string Name,
        string Slug,
        string Domain,
        string TemplateId);

    [HttpPost("tenants")]
    public async Task<IActionResult> CreateTenant(
        [FromBody] CreateTenantRequest req,
        CancellationToken ct)
    {
        // Validate slug first
        var (valid, error) = await provisioning.ValidateSlugAsync(req.Slug);
        if (!valid) return BadRequest(new { error });

        try
        {
            var tenant = await provisioning.ProvisionAsync(
                req.Name, req.Slug, req.Domain, req.TemplateId, ct);

            return Created($"/api/admin/tenants/{tenant.Slug}", new
            {
                tenant.Id,
                tenant.Slug,
                tenant.Status,
                tenant.IsActive
            });
        }
        catch (ProvisioningException ex)
        {
            // Don't expose internal DB errors to client
            return StatusCode(500, new
            {
                error = "Tenant provisioning failed",
                details = ex.Message   // remove in prod
            });
        }
    }
    
    public record UpdateTenantRequest(
    string Name,
    string Domain,
    bool IsActive,
    string Status,
    string TemplateId
);

    // GET: api/admin/tenants
    [HttpGet("tenants")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var tenants = await tenantService.GetAllAsync(ct);

        return Ok(tenants.Select(t => new
        {
            t.Id,
            t.Name,
            t.Slug,
            t.Domain,
            t.TemplateId,
            t.Status,
            t.IsActive,
            t.CreatedAt
        }));
    }

    // GET: api/admin/tenants/{slug}
    [HttpGet("tenants/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var tenant = await tenantService.GetBySlugAsync(slug, ct);

        if (tenant is null)
            return NotFound(new { message = "Tenant not found" });

        return Ok(new
        {
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            tenant.Domain,
            tenant.TemplateId,
            tenant.Status,
            tenant.IsActive,
            tenant.CreatedAt
        });
    }

    // PUT: api/admin/tenants/{slug}
    [HttpPut("tenants/{slug}")]
    public async Task<IActionResult> Update(
        string slug,
        [FromBody] UpdateTenantRequest req,
        CancellationToken ct)
    {
        var tenant = await tenantService.GetBySlugAsync(slug, ct);

        if (tenant is null)
            return NotFound(new { message = "Tenant not found" });

        tenant.Name = req.Name;
        tenant.Domain = req.Domain;
        tenant.IsActive = req.IsActive;
        tenant.Status = req.Status;
        tenant.TemplateId = req.TemplateId;

        var updated = await tenantService.UpdateAsync(tenant, ct);

        return Ok(new
        {
            message = "Tenant updated successfully",
            updated.Id,
            updated.Name,
            updated.Slug,
            updated.Domain,
            updated.TemplateId,
            updated.Status,
            updated.IsActive
        });
    }

    // DELETE: api/admin/tenants/{slug}
    [HttpDelete("tenants/{slug}")]
    public async Task<IActionResult> Delete(string slug, CancellationToken ct)
    {
        var deleted = await tenantService.DeleteAsync(slug, ct);

        if (!deleted)
            return NotFound(new { message = "Tenant not found" });

        return Ok(new
        {
            message = "Tenant deleted successfully"
        });
    }

}