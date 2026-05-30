namespace ErpFactory.Api.Models;

public sealed class MixDesign : BaseEntity
{
    public int MixDesignId { get; set; }
    public string MixName { get; set; } = string.Empty;
    public string? TargetStrength { get; set; }
    public decimal StandardCostPerUnit { get; set; }
    public ICollection<MixIngredient> Ingredients { get; set; } = new List<MixIngredient>();
}