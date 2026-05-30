using System.ComponentModel.DataAnnotations;

namespace ErpFactory.Api.Models;

public sealed class SiteMaterialConsumption
{
    [Key]
    public int SiteConsumptionId { get; set; }

    [Required]
    public int SiteOperationId { get; set; }

    [Required]
    public int MaterialId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal QuantityConsumed { get; set; }

    public SiteOperation? SiteOperation { get; set; }
    public InventoryItem? Material { get; set; }
}
