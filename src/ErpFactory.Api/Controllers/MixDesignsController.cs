

using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,ProjectManager")]
public sealed class MixDesignsController(ErpFactoryDbContext db) : ApiControllerBase
{
    // =========================
    // GET ALL (CLEAN DTO)
    // =========================
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(CancellationToken ct)
    {
        var mixes = await db.MixDesigns
            .AsNoTracking()
            .Select(x => new
            {
                x.MixDesignId,
                x.MixName,
                x.TargetStrength,
                x.StandardCostPerUnit
            })
            .OrderBy(x => x.MixName)
            .ToListAsync(ct);

        return OkResponse<object>(mixes);
    }

    // =========================
    // GET BY ID (WITH INGREDIENTS + NAME)
    // =========================
    [HttpGet("{mixDesignId:int}", Name = nameof(GetMixDesignById))]
    public async Task<ActionResult<ApiResponse<object>>> GetMixDesignById(int mixDesignId, CancellationToken ct)
    {
        var mix = await db.MixDesigns
            .AsNoTracking()
            .Where(x => x.MixDesignId == mixDesignId)
            .Select(x => new
            {
                x.MixDesignId,
                x.MixName,
                x.TargetStrength,
                x.StandardCostPerUnit,

                Ingredients = x.Ingredients.Select(i => new
                {
                    i.IngredientId,
                    i.RawMaterialId,
                    RawMaterialName = i.RawMaterial != null ? i.RawMaterial.ItemName : null,
                    i.StandardQtyPerUnit
                })
            })
            .FirstOrDefaultAsync(ct);

        if (mix is null)
            return NotFoundResponse<object>();

        return OkResponse<object>(mix);
    }

    // =========================
    // CREATE
    // =========================
    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Create(
        CreateMixDesignRequest request,
        CancellationToken ct)
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
            }).ToList() ?? new List<MixIngredient>()
        };

        db.MixDesigns.Add(mix);
        await db.SaveChangesAsync(ct);

        return CreatedResponse(
            nameof(GetMixDesignById),
            new { mixDesignId = mix.MixDesignId },
            new IdResponse(mix.MixDesignId)
        );
    }

    // =========================
    // UPDATE
    // =========================
    [HttpPut("{mixDesignId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(
        int mixDesignId,
        CreateMixDesignRequest request,
        CancellationToken ct)
    {
        var mix = await db.MixDesigns
            .Include(x => x.Ingredients)
            .FirstOrDefaultAsync(x => x.MixDesignId == mixDesignId, ct);

        if (mix is null)
            return NotFoundResponse<object>();

        mix.MixName = request.MixName;
        mix.TargetStrength = request.TargetStrength;
        mix.StandardCostPerUnit = request.StandardCostPerUnit;

        if (request.Ingredients != null)
        {
            db.MixIngredients.RemoveRange(mix.Ingredients);
            mix.Ingredients = request.Ingredients.Select(x => new MixIngredient
            {
                RawMaterialId = x.RawMaterialId,
                StandardQtyPerUnit = x.StandardQtyPerUnit
            }).ToList();
        }

        await db.SaveChangesAsync(ct);

        return OkResponse<object>(mix);
    }

    // =========================
    // INGREDIENTS LIST
    // =========================
    [HttpGet("{mixDesignId:int}/ingredients")]
    public async Task<ActionResult<ApiResponse<object>>> GetIngredients(
        int mixDesignId,
        CancellationToken ct)
    {
        var ingredients = await db.MixIngredients
            .AsNoTracking()
            .Where(x => x.MixDesignId == mixDesignId)
            .Select(i => new
            {
                i.IngredientId,
                i.RawMaterialId,
                RawMaterialName = i.RawMaterial != null ? i.RawMaterial.ItemName : null,
                i.StandardQtyPerUnit
            })
            .ToListAsync(ct);

        return OkResponse<object>(ingredients);
    }

    // =========================
    // ADD INGREDIENT
    // =========================
    [HttpPost("{mixDesignId:int}/ingredients")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> AddIngredient(
        int mixDesignId,
        CreateMixIngredientRequest request,
        CancellationToken ct)
    {
        var ingredient = new MixIngredient
        {
            MixDesignId = mixDesignId,
            RawMaterialId = request.RawMaterialId,
            StandardQtyPerUnit = request.StandardQtyPerUnit
        };

        db.MixIngredients.Add(ingredient);
        await db.SaveChangesAsync(ct);

        return OkResponse(
            new IdResponse(ingredient.IngredientId),
            "Ingredient added successfully"
        );
    }

    // =========================
    // COST ANALYSIS
    // =========================
    [HttpGet("{mixDesignId:int}/cost-analysis")]
    public async Task<ActionResult<ApiResponse<object>>> CostAnalysis(
        int mixDesignId,
        CancellationToken ct)
    {
        var ingredientCosts = await db.MixIngredients
            .AsNoTracking()
            .Where(x => x.MixDesignId == mixDesignId)
            .Include(x => x.RawMaterial)
            .Select(x => new
            {
                x.StandardQtyPerUnit,
                Cost = x.RawMaterial != null ? x.RawMaterial.AverageCost : 0
            })
            .ToListAsync(ct);

        var totalCost = ingredientCosts.Sum(x => x.StandardQtyPerUnit * x.Cost);

        return OkResponse<object>(new
        {
            MixDesignId = mixDesignId,
            EstimatedCostPerUnit = totalCost
        });
    }
}