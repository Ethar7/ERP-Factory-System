using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ErpFactory.Api.Controllers;

[Route("api/v1/mix-ingredients")]
public sealed class MixIngredientsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpPut("{ingredientId:int}")]
    public async Task<ActionResult<ApiResponse<MixIngredient>>> Update(int ingredientId, CreateMixIngredientRequest request, CancellationToken ct)
    {
        var ingredient = await db.MixIngredients.FindAsync([ingredientId], ct);
        if (ingredient is null)
        {
            return NotFoundResponse<MixIngredient>();
        }

        ingredient.RawMaterialId = request.RawMaterialId;
        ingredient.StandardQtyPerUnit = request.StandardQtyPerUnit;
        await db.SaveChangesAsync(ct);
        return OkResponse(ingredient);
    }

    [HttpDelete("{ingredientId:int}")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Delete(int ingredientId, CancellationToken ct)
    {
        var ingredient = await db.MixIngredients.FindAsync([ingredientId], ct);
        if (ingredient is null)
        {
            return NotFoundResponse<IdResponse>();
        }

        db.MixIngredients.Remove(ingredient);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(ingredientId), "Resource deleted successfully");
    }
}
