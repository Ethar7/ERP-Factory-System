using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,ProjectManager")]
public sealed class ProjectItemsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpPut("{projectItemId:int}")]
    public async Task<ActionResult<ApiResponse<ProjectItem>>> Update(int projectItemId, CreateProjectItemRequest request, CancellationToken ct)
    {
        var item = await db.ProjectItems.FindAsync([projectItemId], ct);
        if (item is null)
        {
            return NotFoundResponse<ProjectItem>();
        }

        item.ItemCode = request.ItemCode;
        item.ItemName = request.ItemName;
        item.Unit = request.Unit;
        item.RequiredQuantity = request.RequiredQuantity;
        item.EstimatedUnitPrice = request.EstimatedUnitPrice;
        item.TaxRate = request.TaxRate;

        await db.SaveChangesAsync(ct);
        return OkResponse(item);
    }

    [HttpDelete("{projectItemId:int}")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Delete(int projectItemId, CancellationToken ct)
    {
        var item = await db.ProjectItems.FindAsync([projectItemId], ct);
        if (item is null)
        {
            return NotFoundResponse<IdResponse>();
        }

        db.ProjectItems.Remove(item);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(projectItemId), "Resource deleted successfully");
    }
}
