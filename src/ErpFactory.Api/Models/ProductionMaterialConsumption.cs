namespace ErpFactory.Api.Models;

public sealed class ProductionMaterialConsumption : BaseEntity
{
    public int ConsumptionId { get; set; }
    public int ProductionOrderId { get; set; }
    public int MaterialId { get; set; }
    public decimal ActualQtyConsumed { get; set; }
    public decimal StandardQtyExpected { get; set; }
    public decimal WastageQty { get; private set; }
    public ProductionOrder? ProductionOrder { get; set; }
    public InventoryItem? Material { get; set; }
}