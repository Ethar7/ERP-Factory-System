namespace ErpFactory.Api.Contracts;

public sealed class CreateDeliveryItemRequest
{
    public int ProjectItemId { get; set; }
    public decimal QuantityShipped { get; set; }
    public decimal? QuantityReceived { get; set; }
    public decimal QuantityDamagedInTransit { get; set; }
}

public sealed class ReceiveDeliveryItemRequest
{
    public decimal QuantityReceived { get; set; }
    public decimal QuantityDamagedInTransit { get; set; }
}

public sealed class CreateDeliveryOrderRequest
{
    public int ProjectId { get; set; }
    public string? DriverName { get; set; }
    public string? VehicleNumber { get; set; }
    public string? LoadingTicketNumber { get; set; }
    public string? DeliveryTicketNumber { get; set; }
}

public sealed class UpdateDeliveryStatusRequest
{
    public string DeliveryStatus { get; set; } = string.Empty;
}
