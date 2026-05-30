namespace ErpFactory.Api.Models;

public sealed class ProductionOrder : BaseEntity
{
    public int ProductionOrderId { get; set; }
    public int ProjectId { get; set; }
    public int ProjectItemId { get; set; }
    public int MixDesignId { get; set; }
    public int MoldId { get; set; }
    public DateTime OrderDate { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public decimal TargetQuantity { get; set; }
    public decimal ProducedQuantity { get; set; }
    public decimal GoodQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    public decimal LaborCost { get; set; }
    public decimal MoldDepreciationCost { get; set; }
    public string ProductionStatus { get; set; } = "Setup";
    public bool IsAccountingPosted { get; set; }
    public Project? Project { get; set; }
    public ProjectItem? ProjectItem { get; set; }
    public MixDesign? MixDesign { get; set; }
    public Mold? Mold { get; set; }
    public ICollection<ProductionMaterialConsumption> MaterialConsumption { get; set; } = new List<ProductionMaterialConsumption>();
}