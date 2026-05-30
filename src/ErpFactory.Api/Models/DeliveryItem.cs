namespace ErpFactory.Api.Models;

public sealed class DeliveryItem : BaseEntity
{
    public int DeliveryItemId { get; set; }
    public int DeliveryOrderId { get; set; }
    public int ProjectItemId { get; set; }
    public decimal QuantityShipped { get; set; }
    public decimal? QuantityReceived { get; set; }
    public decimal QuantityDamagedInTransit { get; set; } = 0m;
    public DeliveryOrder? DeliveryOrder { get; set; }
    public ProjectItem? ProjectItem { get; set; }

    public decimal RemainingQuantity => QuantityShipped - (QuantityReceived ?? 0m) - QuantityDamagedInTransit;
}