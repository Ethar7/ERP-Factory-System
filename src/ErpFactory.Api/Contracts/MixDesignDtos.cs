namespace ErpFactory.Api.Contracts;

public sealed class CreateMixIngredientRequest
{
    public int RawMaterialId { get; set; }
    public decimal StandardQtyPerUnit { get; set; }
}

public sealed class CreateMixDesignRequest
{
    public string MixName { get; set; } = string.Empty;
    public string? TargetStrength { get; set; }
    public decimal StandardCostPerUnit { get; set; }
    public IReadOnlyCollection<CreateMixIngredientRequest> Ingredients { get; set; } = Array.Empty<CreateMixIngredientRequest>();
}