using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

public sealed class ProjectMoldsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet("/api/v1/projects/{projectId:int}/molds")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProjectMold>>>> GetByProject(int projectId, CancellationToken ct)
    {
        var allocations = await db.ProjectMolds
            .AsNoTracking()
            .Include(x => x.Mold)
            .Where(x => x.ProjectId == projectId)
            .ToListAsync(ct);

        return OkCollection(allocations);
    }

    [HttpPost("/api/v1/projects/{projectId:int}/molds")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Allocate(int projectId, AllocateMoldRequest request, CancellationToken ct)
    {
        var allocation = new ProjectMold
        {
            ProjectId = projectId,
            MoldId = request.MoldId,
            AllocQuantity = request.AllocQuantity
        };

        db.ProjectMolds.Add(allocation);

        var mold = await db.Molds.FindAsync([request.MoldId], ct);
        if (mold is not null)
        {
            mold.MoldStatus = "Allocated";
        }

        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(allocation.ProjectMoldId), "Resource created successfully");
    }

    [HttpDelete("/api/v1/project-molds/{projectMoldId:int}")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Delete(int projectMoldId, CancellationToken ct)
    {
        var allocation = await db.ProjectMolds.FindAsync([projectMoldId], ct);
        if (allocation is null)
        {
            return NotFoundResponse<IdResponse>();
        }

        db.ProjectMolds.Remove(allocation);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(projectMoldId), "Resource deleted successfully");
    }
}
