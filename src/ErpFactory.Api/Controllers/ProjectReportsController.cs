using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

public sealed class ProjectReportsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet("/api/v1/projects/{projectId:int}/inventory-transactions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<Models.InventoryTransaction>>>> ProjectInventoryTransactions(int projectId, CancellationToken ct)
    {
        var rows = await db.InventoryTransactions.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(ct);

        return OkCollection(rows);
    }

    [HttpGet("/api/v1/projects/{projectId:int}/delivery-summary")]
    public async Task<ActionResult<ApiResponse<object>>> DeliverySummary(int projectId, CancellationToken ct)
    {
        var rows = await db.DeliveryItems.AsNoTracking()
            .Where(x => x.DeliveryOrder != null && x.DeliveryOrder.ProjectId == projectId)
            .GroupBy(x => x.DeliveryOrder!.ProjectId)
            .Select(g => new
            {
                ProjectId = g.Key,
                Shipped = g.Sum(x => x.QuantityShipped),
                Received = g.Sum(x => x.QuantityReceived ?? 0),
                Damaged = g.Sum(x => x.QuantityDamagedInTransit)
            })
            .FirstOrDefaultAsync(ct);

        return OkResponse<object>(rows ?? new { ProjectId = projectId, Shipped = 0m, Received = 0m, Damaged = 0m });
    }

    [HttpGet("/api/v1/projects/{projectId:int}/installation-summary")]
    public async Task<ActionResult<ApiResponse<object>>> InstallationSummary(int projectId, CancellationToken ct)
    {
        var rows = await db.SiteOperations.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .GroupBy(x => x.ProjectId)
            .Select(g => new
            {
                ProjectId = g.Key,
                Installed = g.Sum(x => x.InstalledQuantity),
                SiteCost = g.Sum(x => x.SupervisorLaborCost + x.DailyExpenses)
            })
            .FirstOrDefaultAsync(ct);

        return OkResponse<object>(rows ?? new { ProjectId = projectId, Installed = 0m, SiteCost = 0m });
    }
}
