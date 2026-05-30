namespace ErpFactory.Api.Contracts;

public sealed class CreateProductionOrderRequest
{
    public int ProjectId { get; set; }
    public int ProjectItemId { get; set; }
    public int MixDesignId { get; set; }
    public int MoldId { get; set; }
    public string? BatchNumber { get; set; }
    public decimal TargetQuantity { get; set; }
    public decimal LaborCost { get; set; }
    public decimal MoldDepreciationCost { get; set; }
}

public sealed class UpdateProductionStatusRequest
{
    public string ProductionStatus { get; set; } = string.Empty;
}

public sealed class RecordQualityCheckRequest
{
    public decimal ProducedQuantity { get; set; }
    public decimal GoodQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    public decimal LaborCost { get; set; }
    public decimal MoldDepreciationCost { get; set; }
}

public sealed class RecordProductionConsumptionRequest
{
    public int MaterialId { get; set; }
    public decimal ActualQtyConsumed { get; set; }
    public decimal StandardQtyExpected { get; set; }
}
