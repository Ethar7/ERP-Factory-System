namespace ErpFactory.Api.Contracts;

public sealed class CreateInventoryItemRequest
{
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal AverageCost { get; set; }
}

public sealed class CreateInventoryTransactionRequest
{
    public int ItemId { get; set; }
    public int? ProjectId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? Notes { get; set; }
}