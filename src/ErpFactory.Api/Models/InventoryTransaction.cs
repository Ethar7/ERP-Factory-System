using System.ComponentModel.DataAnnotations;

namespace ErpFactory.Api.Models;

public sealed class InventoryTransaction
{
    [Key]
    public int TransactionId { get; set; }

    [Required]
    public int ItemId { get; set; }

    public int? ProjectId { get; set; }

    [Required]
    [StringLength(50)]
    public string TransactionType { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitCost { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; }

    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public InventoryItem? Item { get; set; }
    public Project? Project { get; set; }
}
