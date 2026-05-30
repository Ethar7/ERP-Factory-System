using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,ProjectManager")]
public sealed class SiteOperationsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<SiteOperation>>>> GetAll([FromQuery] int? projectId, CancellationToken ct)
    {
        var query = db.SiteOperations.AsNoTracking().AsQueryable();
        if (projectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == projectId.Value);
        }

        return OkCollection(await query.OrderByDescending(x => x.OperationDate).ToListAsync(ct));
    }

    [HttpGet("{siteOperationId:int}", Name = nameof(GetSiteOperationById))]
    public async Task<ActionResult<ApiResponse<SiteOperation>>> GetSiteOperationById(int siteOperationId, CancellationToken ct)
    {
        var operation = await db.SiteOperations.AsNoTracking().Include(x => x.Consumption).FirstOrDefaultAsync(x => x.SiteOperationId == siteOperationId, ct);
        return operation is null ? NotFoundResponse<SiteOperation>() : OkResponse(operation);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Create(CreateSiteOperationRequest request, CancellationToken ct)
    {
        var operation = new SiteOperation
        {
            ProjectId = request.ProjectId,
            ProjectItemId = request.ProjectItemId,
            InstalledQuantity = request.InstalledQuantity,
            SupervisorLaborCost = request.SupervisorLaborCost,
            DailyExpenses = request.DailyExpenses
        };

        db.SiteOperations.Add(operation);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return FailResponse<IdResponse>("Database update failed: " + ex.Message);
        }

        return CreatedResponse(nameof(GetSiteOperationById), new { siteOperationId = operation.SiteOperationId }, new IdResponse(operation.SiteOperationId));
    }

    [HttpPut("{siteOperationId:int}")]
    public async Task<ActionResult<ApiResponse<SiteOperation>>> Update(int siteOperationId, CreateSiteOperationRequest request, CancellationToken ct)
    {
        var operation = await db.SiteOperations.FirstOrDefaultAsync(x => x.SiteOperationId == siteOperationId, ct);
        if (operation is null)
        {
            return NotFoundResponse<SiteOperation>();
        }

        operation.ProjectId = request.ProjectId;
        operation.ProjectItemId = request.ProjectItemId;
        operation.InstalledQuantity = request.InstalledQuantity;
        operation.SupervisorLaborCost = request.SupervisorLaborCost;
        operation.DailyExpenses = request.DailyExpenses;

        await db.SaveChangesAsync(ct);
        return OkResponse(operation);
    }

    [HttpGet("{siteOperationId:int}/consumption")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<SiteMaterialConsumption>>>> GetConsumption(int siteOperationId, CancellationToken ct) =>
        OkCollection(await db.SiteMaterialConsumption.AsNoTracking().Where(x => x.SiteOperationId == siteOperationId).ToListAsync(ct));

    [HttpPost("{siteOperationId:int}/consumption")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> AddConsumption(int siteOperationId, CreateSiteConsumptionRequest request, CancellationToken ct)
    {
        var consumption = new SiteMaterialConsumption
        {
            SiteOperationId = siteOperationId,
            MaterialId = request.MaterialId,
            QuantityConsumed = request.QuantityConsumed
        };

        db.SiteMaterialConsumption.Add(consumption);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(consumption.SiteConsumptionId), "Resource created successfully");
    }

    [HttpGet("project/{projectId:int}/cost-summary")]
    public async Task<ActionResult<ApiResponse<ProjectCostSummary>>> ProjectCostSummary(int projectId, CancellationToken ct)
    {
        var summary = await db.ProjectCostSummary.AsNoTracking().FirstOrDefaultAsync(x => x.ProjectId == projectId, ct);
        return summary is null ? NotFoundResponse<ProjectCostSummary>() : OkResponse(summary);
    }
}
