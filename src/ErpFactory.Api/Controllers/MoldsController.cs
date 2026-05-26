using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

public sealed class MoldsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<Mold>>>> GetAll(CancellationToken ct) =>
        OkCollection(await db.Molds.AsNoTracking().OrderBy(x => x.MoldName).ToListAsync(ct));

    [HttpGet("{moldId:int}", Name = nameof(GetMoldById))]
    public async Task<ActionResult<ApiResponse<Mold>>> GetMoldById(int moldId, CancellationToken ct)
    {
        var mold = await db.Molds.AsNoTracking().FirstOrDefaultAsync(x => x.MoldId == moldId, ct);
        return mold is null ? NotFoundResponse<Mold>() : OkResponse(mold);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Create(CreateMoldRequest request, CancellationToken ct)
    {
        var mold = new Mold
        {
            MoldName = request.MoldName,
            CostToBuild = request.CostToBuild,
            ExpectedLifespanUses = request.ExpectedLifespanUses,
            MoldStatus = request.MoldStatus ?? "Available"
        };

        db.Molds.Add(mold);
        await db.SaveChangesAsync(ct);
        return CreatedResponse(nameof(GetMoldById), new { moldId = mold.MoldId }, new IdResponse(mold.MoldId));
    }

    [HttpPut("{moldId:int}")]
    public async Task<ActionResult<ApiResponse<Mold>>> Update(int moldId, CreateMoldRequest request, CancellationToken ct)
    {
        var mold = await db.Molds.FindAsync([moldId], ct);
        if (mold is null)
        {
            return NotFoundResponse<Mold>();
        }

        mold.MoldName = request.MoldName;
        mold.CostToBuild = request.CostToBuild;
        mold.ExpectedLifespanUses = request.ExpectedLifespanUses;
        mold.MoldStatus = request.MoldStatus ?? mold.MoldStatus;

        await db.SaveChangesAsync(ct);
        return OkResponse(mold);
    }

    [HttpPatch("{moldId:int}/status")]
    public async Task<ActionResult<ApiResponse<Mold>>> UpdateStatus(int moldId, UpdateMoldStatusRequest request, CancellationToken ct)
    {
        var mold = await db.Molds.FindAsync([moldId], ct);
        if (mold is null)
        {
            return NotFoundResponse<Mold>();
        }

        mold.MoldStatus = request.MoldStatus;
        await db.SaveChangesAsync(ct);
        return OkResponse(mold);
    }
}
