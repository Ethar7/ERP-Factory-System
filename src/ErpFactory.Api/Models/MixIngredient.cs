namespace ErpFactory.Api.Models;

public sealed class MixIngredient : BaseEntity
{
    public int IngredientId { get; set; }
    public int MixDesignId { get; set; }
    public int RawMaterialId { get; set; }
    public decimal StandardQtyPerUnit { get; set; }
    public MixDesign? MixDesign { get; set; }
    public InventoryItem? RawMaterial { get; set; }
}