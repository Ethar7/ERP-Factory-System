namespace ErpFactory.Api.Models;

public sealed class SiteMaterialConsumption : BaseEntity
{
    public int SiteConsumptionId { get; set; }
    public int SiteOperationId { get; set; }
    public int MaterialId { get; set; }
    public decimal QuantityConsumed { get; set; }

    public SiteOperation? SiteOperation { get; set; }
    public InventoryItem? Material { get; set; }
}