namespace ErpFactory.Api.Models;

public sealed class InventoryTransaction : BaseEntity
{
    public int TransactionId { get; set; }
    public int ItemId { get; set; }
    public int? ProjectId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public InventoryItem? Item { get; set; }
    public Project? Project { get; set; }
}