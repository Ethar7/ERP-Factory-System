using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

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
            StartDate = request.StartDate ?? DateTime.Now,
            TotalEstimatedBudget = request.TotalEstimatedBudget,
            Items = request.Items?.Select(ToProjectItem).ToList() ?? []
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);
        return CreatedResponse(nameof(GetProjectById), new { projectId = project.ProjectId }, new IdResponse(project.ProjectId));
    }

    [HttpPut("{projectId:int}")]
    public async Task<ActionResult<ApiResponse<Project>>> Update(int projectId, CreateProjectRequest request, CancellationToken ct)
    {
        var project = await db.Projects.FindAsync([projectId], ct);
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
        var project = await db.Projects.FindAsync([projectId], ct);
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
}
