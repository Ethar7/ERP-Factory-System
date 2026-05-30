using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpFactory.Api.DTOS;
using Microsoft.AspNetCore.Authorization;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,ProjectManager,Accountant")]
public sealed class ReportsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet("project-cost-summary")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProjectCostSummary>>>> ProjectCostSummary(CancellationToken ct) =>
        OkCollection(await db.ProjectCostSummary.AsNoTracking().OrderBy(x => x.ProjectName).ToListAsync(ct));

    [HttpGet("projects/{projectId:int}/cost-summary")]
    public async Task<ActionResult<ApiResponse<ProjectCostSummary>>> ProjectCostSummaryById(int projectId, CancellationToken ct)
    {
        var summary = await db.ProjectCostSummary.AsNoTracking().FirstOrDefaultAsync(x => x.ProjectId == projectId, ct);
        return summary is null ? NotFoundResponse<ProjectCostSummary>() : OkResponse(summary);
    }

    [HttpGet("projects/{projectId:int}/profitability")]
    public async Task<ActionResult<ApiResponse<object>>> Profitability(int projectId, CancellationToken ct)
    {
        var summary = await db.ProjectCostSummary.AsNoTracking().FirstOrDefaultAsync(x => x.ProjectId == projectId, ct);
        if (summary is null)
        {
            return NotFoundResponse<object>();
        }

        return OkResponse<object>(new
        {
            summary.ProjectId,
            summary.ProjectName,
            EstimatedBudget = summary.TotalEstimatedBudget,
            summary.ProductionDirectCost,
            summary.SiteDirectCost,
            summary.TotalDirectCost,
            EstimatedProfit = summary.TotalEstimatedBudget - summary.TotalDirectCost
        });
    }

    [HttpGet("projects/{projectId:int}/production-progress")]
    public async Task<ActionResult<ApiResponse<object>>> ProductionProgress(int projectId, CancellationToken ct)
    {
        var rows = await db.ProductionOrders.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .GroupBy(x => x.ProjectId)
            .Select(g => new
            {
                ProjectId = g.Key,
                TargetQuantity = g.Sum(x => x.TargetQuantity),
                ProducedQuantity = g.Sum(x => x.ProducedQuantity),
                GoodQuantity = g.Sum(x => x.GoodQuantity),
                RejectedQuantity = g.Sum(x => x.RejectedQuantity)
            })
            .FirstOrDefaultAsync(ct);

        return OkResponse<object>(rows ?? new { ProjectId = projectId, TargetQuantity = 0m, ProducedQuantity = 0m, GoodQuantity = 0m, RejectedQuantity = 0m });
    }

    [HttpGet("projects/{projectId:int}/delivery-progress")]
    public async Task<ActionResult<ApiResponse<object>>> DeliveryProgress(int projectId, CancellationToken ct)
    {
        var rows = await db.DeliveryItems.AsNoTracking()
            .Where(x => x.DeliveryOrder != null && x.DeliveryOrder.ProjectId == projectId)
            .GroupBy(x => x.DeliveryOrder!.ProjectId)
            .Select(g => new
            {
                ProjectId = g.Key,
                QuantityShipped = g.Sum(x => x.QuantityShipped),
                QuantityReceived = g.Sum(x => x.QuantityReceived ?? 0),
                QuantityDamagedInTransit = g.Sum(x => x.QuantityDamagedInTransit)
            })
            .FirstOrDefaultAsync(ct);

        return OkResponse<object>(rows ?? new { ProjectId = projectId, QuantityShipped = 0m, QuantityReceived = 0m, QuantityDamagedInTransit = 0m });
    }

    [HttpGet("projects/{projectId:int}/installation-progress")]
    public async Task<ActionResult<ApiResponse<object>>> InstallationProgress(int projectId, CancellationToken ct)
    {
        var rows = await db.SiteOperations.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .GroupBy(x => x.ProjectId)
            .Select(g => new
            {
                ProjectId = g.Key,
                InstalledQuantity = g.Sum(x => x.InstalledQuantity),
                SiteCost = g.Sum(x => x.SupervisorLaborCost + x.DailyExpenses)
            })
            .FirstOrDefaultAsync(ct);

        return OkResponse<object>(rows ?? new { ProjectId = projectId, InstalledQuantity = 0m, SiteCost = 0m });
    }

    [HttpGet("projects/{projectId:int}/material-variance")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> MaterialVariance(int projectId, CancellationToken ct)
    {
        var rows = await db.ProductionMaterialConsumption.AsNoTracking()
            .Where(x => x.ProductionOrder != null && x.ProductionOrder.ProjectId == projectId)
            .Select(x => new
            {
                x.ConsumptionId,
                x.ProductionOrderId,
                x.MaterialId,
                x.ActualQtyConsumed,
                x.StandardQtyExpected,
                x.WastageQty
            })
            .Cast<object>()
            .ToListAsync(ct);

        return OkCollection(rows);
    }

    [HttpGet("projects/{projectId:int}/mold-cost")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> MoldCost(int projectId, CancellationToken ct)
    {
        var rows = await db.ProductionOrders.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .GroupBy(x => x.MoldId)
            .Select(g => new
            {
                MoldId = g.Key,
                Uses = g.Count(),
                MoldDepreciationCost = g.Sum(x => x.MoldDepreciationCost)
            })
            .Cast<object>()
            .ToListAsync(ct);

        return OkCollection(rows);
    }

    [HttpGet("journal-entry-balance")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<JournalEntryBalance>>>> JournalEntryBalance(CancellationToken ct) =>
        OkCollection(await db.JournalEntryBalance.AsNoTracking().ToListAsync(ct));
}
