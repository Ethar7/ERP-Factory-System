using System.Collections.Generic;

namespace ErpFactory.Api.Models;

public sealed class DeliveryOrder
{
    public int DeliveryOrderId { get; set; }
    public int ProjectId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string? DriverName { get; set; }
    public string? VehicleNumber { get; set; }
    public string? LoadingTicketNumber { get; set; }
    public string? DeliveryTicketNumber { get; set; }
    public string DeliveryStatus { get; set; } = "InTransit";
    public bool IsInvoiced { get; set; }
    public Project? Project { get; set; }
    public ICollection<DeliveryItem> Items { get; set; } = new List<DeliveryItem>();
}