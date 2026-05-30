using System.ComponentModel.DataAnnotations;

namespace ErpFactory.Api.Models;

public sealed class ProductionMaterialConsumption
{
    [Key]
    public int ConsumptionId { get; set; }

    [Required]
    public int ProductionOrderId { get; set; }

    [Required]
    public int MaterialId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ActualQtyConsumed { get; set; }

    [Range(0, double.MaxValue)]
    public decimal StandardQtyExpected { get; set; }

    public decimal WastageQty { get; private set; }

    public ProductionOrder? ProductionOrder { get; set; }
    public InventoryItem? Material { get; set; }
}
