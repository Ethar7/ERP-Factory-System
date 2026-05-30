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
public sealed class ProjectsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<Project>>>> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] string? status,
        CancellationToken ct)
    {
        var query = db.Projects.AsNoTracking().Include(x => x.Customer).AsQueryable();

        if (customerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == customerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.ProjectStatus == status);
        }

        return OkCollection(await query.OrderByDescending(x => x.CreatedAt).ToListAsync(ct));
    }

    [HttpGet("{projectId:int}", Name = nameof(GetProjectById))]
    public async Task<ActionResult<ApiResponse<Project>>> GetProjectById(int projectId, CancellationToken ct)
    {
        var project = await db.Projects
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.ProjectId == projectId, ct);

        return project is null ? NotFoundResponse<Project>() : OkResponse(project);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Create(CreateProjectRequest request, CancellationToken ct)
    {
        var project = new Project
        {
            ProjectName = request.ProjectName,
            CustomerId = request.CustomerId,
            StartDate = request.StartDate ?? DateTime.UtcNow,
            TotalEstimatedBudget = request.TotalEstimatedBudget,
            Items = request.Items?.Select(ToProjectItem).ToList() ?? new List<ProjectItem>()
        };

        db.Projects.Add(project);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return FailResponse<IdResponse>("Database update failed: " + ex.Message);
        }

        return CreatedResponse(nameof(GetProjectById), new { projectId = project.ProjectId }, new IdResponse(project.ProjectId));
    }

    [HttpPut("{projectId:int}")]
    public async Task<ActionResult<ApiResponse<Project>>> Update(int projectId, CreateProjectRequest request, CancellationToken ct)
    {
        var project = await db.Projects.FirstOrDefaultAsync(x => x.ProjectId == projectId, ct);
        if (project is null)
        {
            return NotFoundResponse<Project>();
        }

        project.ProjectName = request.ProjectName;
        project.CustomerId = request.CustomerId;
        project.StartDate = request.StartDate ?? project.StartDate;
        project.TotalEstimatedBudget = request.TotalEstimatedBudget;

        await db.SaveChangesAsync(ct);
        return OkResponse(project);
    }

    [HttpPatch("{projectId:int}/status")]
    public async Task<ActionResult<ApiResponse<Project>>> UpdateStatus(int projectId, UpdateProjectStatusRequest request, CancellationToken ct)
    {
        var project = await db.Projects.FirstOrDefaultAsync(x => x.ProjectId == projectId, ct);
        if (project is null)
        {
            return NotFoundResponse<Project>();
        }

        project.ProjectStatus = request.ProjectStatus;
        await db.SaveChangesAsync(ct);
        return OkResponse(project);
    }

    [HttpGet("{projectId:int}/items")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProjectItem>>>> GetItems(int projectId, CancellationToken ct)
    {
        var items = await db.ProjectItems.AsNoTracking().Where(x => x.ProjectId == projectId).ToListAsync(ct);
        return OkCollection(items);
    }

    [HttpPost("{projectId:int}/items")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<IdResponse>>>> AddItems(
        int projectId,
        IReadOnlyCollection<CreateProjectItemRequest> request,
        CancellationToken ct)
    {
        var exists = await db.Projects.AnyAsync(x => x.ProjectId == projectId, ct);
        if (!exists)
        {
            return NotFoundResponse<IReadOnlyCollection<IdResponse>>("Project was not found");
        }

        var items = request.Select(x =>
        {
            var item = ToProjectItem(x);
            item.ProjectId = projectId;
            return item;
        }).ToList();

        db.ProjectItems.AddRange(items);
        await db.SaveChangesAsync(ct);
        return OkCollection(items.Select(x => new IdResponse(x.ProjectItemId)).ToList());
    }

    private static ProjectItem ToProjectItem(CreateProjectItemRequest request) => new()
    {
        ItemCode = request.ItemCode,
        ItemName = request.ItemName,
        Unit = request.Unit,
        RequiredQuantity = request.RequiredQuantity,
        EstimatedUnitPrice = request.EstimatedUnitPrice,
        TaxRate = request.TaxRate
    };

    [HttpGet("{projectId:int}/summary")]
    public async Task<ActionResult<ApiResponse<ProjectCostSummary>>> Summary(int projectId, CancellationToken ct)
    {
        var summary = await db.ProjectCostSummary.AsNoTracking().FirstOrDefaultAsync(x => x.ProjectId == projectId, ct);
        return summary is null ? NotFoundResponse<ProjectCostSummary>() : OkResponse(summary);
    }

    [HttpGet("{projectId:int}/dashboard")]
    public async Task<ActionResult<ApiResponse<object>>> Dashboard(int projectId, CancellationToken ct)
    {
        var project = await db.Projects.AsNoTracking().FirstOrDefaultAsync(x => x.ProjectId == projectId, ct);
        if (project is null)
        {
            return NotFoundResponse<object>();
        }

        var production = await db.ProductionOrders.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .GroupBy(x => x.ProjectId)
            .Select(g => new
            {
                TargetQuantity = g.Sum(x => x.TargetQuantity),
                ProducedQuantity = g.Sum(x => x.ProducedQuantity),
                GoodQuantity = g.Sum(x => x.GoodQuantity),
                RejectedQuantity = g.Sum(x => x.RejectedQuantity)
            })
            .FirstOrDefaultAsync(ct);

        var delivery = await db.DeliveryItems.AsNoTracking()
            .Where(x => x.DeliveryOrder != null && x.DeliveryOrder.ProjectId == projectId)
            .GroupBy(x => x.DeliveryOrder!.ProjectId)
            .Select(g => new
            {
                QuantityShipped = g.Sum(x => x.QuantityShipped),
                QuantityReceived = g.Sum(x => x.QuantityReceived ?? 0),
                QuantityDamagedInTransit = g.Sum(x => x.QuantityDamagedInTransit)
            })
            .FirstOrDefaultAsync(ct);

        var installation = await db.SiteOperations.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .GroupBy(x => x.ProjectId)
            .Select(g => new
            {
                InstalledQuantity = g.Sum(x => x.InstalledQuantity),
                SiteCost = g.Sum(x => x.SupervisorLaborCost + x.DailyExpenses)
            })
            .FirstOrDefaultAsync(ct);

        var dashboardData = new
        {
            project.ProjectId,
            project.ProjectName,
            EstimatedBudget = project.TotalEstimatedBudget,
            Production = production ?? new { TargetQuantity = 0m, ProducedQuantity = 0m, GoodQuantity = 0m, RejectedQuantity = 0m },
            Delivery = delivery ?? new { QuantityShipped = 0m, QuantityReceived = 0m, QuantityDamagedInTransit = 0m },
            Installation = installation ?? new { InstalledQuantity = 0m, SiteCost = 0m }
        };

        return OkResponse<object>(dashboardData);
    }
}