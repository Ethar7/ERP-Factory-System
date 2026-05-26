using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

[Route("api/v1/mix-designs")]
public sealed class MixDesignsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<MixDesign>>>> GetAll(CancellationToken ct) =>
        OkCollection(await db.MixDesigns.AsNoTracking().OrderBy(x => x.MixName).ToListAsync(ct));

    [HttpGet("{mixDesignId:int}", Name = nameof(GetMixDesignById))]
    public async Task<ActionResult<ApiResponse<MixDesign>>> GetMixDesignById(int mixDesignId, CancellationToken ct)
    {
        var mix = await db.MixDesigns.AsNoTracking().Include(x => x.Ingredients).FirstOrDefaultAsync(x => x.MixDesignId == mixDesignId, ct);
        return mix is null ? NotFoundResponse<MixDesign>() : OkResponse(mix);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Create(CreateMixDesignRequest request, CancellationToken ct)
    {
        var mix = new MixDesign
        {
            MixName = request.MixName,
            TargetStrength = request.TargetStrength,
            StandardCostPerUnit = request.StandardCostPerUnit,
            Ingredients = request.Ingredients?.Select(x => new MixIngredient
            {
                RawMaterialId = x.RawMaterialId,
                StandardQtyPerUnit = x.StandardQtyPerUnit
            }).ToList() ?? []
        };

        db.MixDesigns.Add(mix);
        await db.SaveChangesAsync(ct);
        return CreatedResponse(nameof(GetMixDesignById), new { mixDesignId = mix.MixDesignId }, new IdResponse(mix.MixDesignId));
    }

    [HttpPut("{mixDesignId:int}")]
    public async Task<ActionResult<ApiResponse<MixDesign>>> Update(int mixDesignId, CreateMixDesignRequest request, CancellationToken ct)
    {
        var mix = await db.MixDesigns.FindAsync([mixDesignId], ct);
        if (mix is null)
        {
            return NotFoundResponse<MixDesign>();
        }

        mix.MixName = request.MixName;
        mix.TargetStrength = request.TargetStrength;
        mix.StandardCostPerUnit = request.StandardCostPerUnit;

        await db.SaveChangesAsync(ct);
        return OkResponse(mix);
    }

    [HttpGet("{mixDesignId:int}/ingredients")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<MixIngredient>>>> GetIngredients(int mixDesignId, CancellationToken ct) =>
        OkCollection(await db.MixIngredients.AsNoTracking().Where(x => x.MixDesignId == mixDesignId).ToListAsync(ct));

    [HttpPost("{mixDesignId:int}/ingredients")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> AddIngredient(int mixDesignId, CreateMixIngredientRequest request, CancellationToken ct)
    {
        var ingredient = new MixIngredient
        {
            MixDesignId = mixDesignId,
            RawMaterialId = request.RawMaterialId,
            StandardQtyPerUnit = request.StandardQtyPerUnit
        };

        db.MixIngredients.Add(ingredient);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(ingredient.IngredientId), "Resource created successfully");
    }
}
