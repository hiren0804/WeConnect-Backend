using WeConnect.Application.DTOs;
using WeConnect.Domain.Entities;
using WeConnect.Domain.Interfaces;

namespace WeConnect.Application.Services;

public class WidgetService
{
    public async Task<IEnumerable<WidgetDetailDto>> GetAllAsync(
        IWidgetRepository repo, CancellationToken ct = default)
    {
        var items = await repo.GetAllAsync(ct);
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<WidgetDetailDto>> GetByPageIdAsync(
        IWidgetRepository repo, Guid pageId, CancellationToken ct = default)
    {
        var items = await repo.GetByPageIdAsync(pageId, ct);
        return items.Select(MapToDto);
    }

    public async Task<WidgetDetailDto?> GetByIdAsync(
        IWidgetRepository repo, Guid id, CancellationToken ct = default)
    {
        var item = await repo.GetByIdAsync(id, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<WidgetDetailDto> CreateAsync(
        IWidgetRepository repo, CreateWidgetRequest request, CancellationToken ct = default)
    {
        var item = new TenantWidget
        {
            Id = Guid.NewGuid(),
            PageId = request.PageId,
            WidgetKey = request.WidgetKey,
            WidgetType = request.WidgetType,
            Col = request.Col,
            Height = request.Height,
            DisplayOrder = request.DisplayOrder,
            IsVisible = request.IsVisible
        };

        var created = await repo.CreateAsync(item, ct);
        return MapToDto(created);
    }

    public async Task<WidgetDetailDto?> UpdateAsync(
        IWidgetRepository repo, Guid id, UpdateWidgetRequest request, CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null) return null;

        existing.PageId = request.PageId;
        existing.WidgetKey = request.WidgetKey;
        existing.WidgetType = request.WidgetType;
        existing.Col = request.Col;
        existing.Height = request.Height;
        existing.DisplayOrder = request.DisplayOrder;
        existing.IsVisible = request.IsVisible;

        var updated = await repo.UpdateAsync(existing, ct);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(
        IWidgetRepository repo, Guid id, CancellationToken ct = default)
    {
        return await repo.DeleteAsync(id, ct);
    }

    private static WidgetDetailDto MapToDto(TenantWidget w) => new(
        w.Id,
        w.PageId,
        w.WidgetKey,
        w.WidgetType,
        w.Col,
        w.Height,
        w.DisplayOrder,
        w.IsVisible
    );
}

