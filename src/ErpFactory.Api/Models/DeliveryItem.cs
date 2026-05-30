using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpFactory.Api.Models;

public sealed class DeliveryItem
{
    [Key]
    public int DeliveryItemId { get; set; }

    [Required]
    public int DeliveryOrderId { get; set; }

    [Required]
    public int ProjectItemId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal QuantityShipped { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? QuantityReceived { get; set; }

    [Range(0, double.MaxValue)]
    public decimal QuantityDamagedInTransit { get; set; } = 0m;

    public DeliveryOrder? DeliveryOrder { get; set; }
    public ProjectItem? ProjectItem { get; set; }

    [NotMapped]
    public decimal RemainingQuantity => QuantityShipped - (QuantityReceived ?? 0m) - QuantityDamagedInTransit;
}
