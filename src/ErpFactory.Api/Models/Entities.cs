namespace ErpFactory.Api.Models;

public sealed class ChartOfAccount
{
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public ICollection<Customer> Customers { get; set; } = [];
    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = [];
}

public sealed class Customer
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int? AccountId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ChartOfAccount? Account { get; set; }
    public ICollection<Project> Projects { get; set; } = [];
}

public sealed class Project
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ProjectStatus { get; set; } = "Draft";
    public decimal TotalEstimatedBudget { get; set; }
    public DateTime CreatedAt { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<ProjectItem> Items { get; set; } = [];
    public ICollection<ProjectMold> ProjectMolds { get; set; } = [];
    public ICollection<ProductionOrder> ProductionOrders { get; set; } = [];
    public ICollection<DeliveryOrder> DeliveryOrders { get; set; } = [];
    public ICollection<SiteOperation> SiteOperations { get; set; } = [];
}

public sealed class ProjectItem
{
    public int ProjectItemId { get; set; }
    public int ProjectId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal EstimatedUnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal TotalPriceWithTax { get; private set; }
    public Project? Project { get; set; }
}

public sealed class InventoryItem
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal AverageCost { get; set; }
    public ICollection<InventoryTransaction> Transactions { get; set; } = [];
}

public sealed class InventoryTransaction
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

public sealed class Mold
{
    public int MoldId { get; set; }
    public string MoldName { get; set; } = string.Empty;
    public decimal CostToBuild { get; set; }
    public int ExpectedLifespanUses { get; set; }
    public int CurrentUsesCount { get; set; }
    public string MoldStatus { get; set; } = "Available";
    public ICollection<ProjectMold> ProjectMolds { get; set; } = [];
}

public sealed class ProjectMold
{
    public int ProjectMoldId { get; set; }
    public int ProjectId { get; set; }
    public int MoldId { get; set; }
    public int AllocQuantity { get; set; }
    public DateTime AllocatedAt { get; set; }
    public Project? Project { get; set; }
    public Mold? Mold { get; set; }
}

public sealed class MixDesign
{
    public int MixDesignId { get; set; }
    public string MixName { get; set; } = string.Empty;
    public string? TargetStrength { get; set; }
    public decimal StandardCostPerUnit { get; set; }
    public ICollection<MixIngredient> Ingredients { get; set; } = [];
}

public sealed class MixIngredient
{
    public int IngredientId { get; set; }
    public int MixDesignId { get; set; }
    public int RawMaterialId { get; set; }
    public decimal StandardQtyPerUnit { get; set; }
    public MixDesign? MixDesign { get; set; }
    public InventoryItem? RawMaterial { get; set; }
}

public sealed class ProductionOrder
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
    public ICollection<ProductionMaterialConsumption> MaterialConsumption { get; set; } = [];
}

public sealed class ProductionMaterialConsumption
{
    public int ConsumptionId { get; set; }
    public int ProductionOrderId { get; set; }
    public int MaterialId { get; set; }
    public decimal ActualQtyConsumed { get; set; }
    public decimal StandardQtyExpected { get; set; }
    public decimal WastageQty { get; private set; }
    public ProductionOrder? ProductionOrder { get; set; }
    public InventoryItem? Material { get; set; }
}

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
    public ICollection<DeliveryItem> Items { get; set; } = [];
}

public sealed class DeliveryItem
{
    public int DeliveryItemId { get; set; }
    public int DeliveryOrderId { get; set; }
    public int ProjectItemId { get; set; }
    public decimal QuantityShipped { get; set; }
    public decimal? QuantityReceived { get; set; }
    public decimal QuantityDamagedInTransit { get; set; }
    public DeliveryOrder? DeliveryOrder { get; set; }
    public ProjectItem? ProjectItem { get; set; }
}

public sealed class SiteOperation
{
    public int SiteOperationId { get; set; }
    public int ProjectId { get; set; }
    public int ProjectItemId { get; set; }
    public DateTime OperationDate { get; set; }
    public decimal InstalledQuantity { get; set; }
    public decimal SupervisorLaborCost { get; set; }
    public decimal DailyExpenses { get; set; }
    public Project? Project { get; set; }
    public ProjectItem? ProjectItem { get; set; }
    public ICollection<SiteMaterialConsumption> Consumption { get; set; } = [];
}

public sealed class SiteMaterialConsumption
{
    public int SiteConsumptionId { get; set; }
    public int SiteOperationId { get; set; }
    public int MaterialId { get; set; }
    public decimal QuantityConsumed { get; set; }
    public SiteOperation? SiteOperation { get; set; }
    public InventoryItem? Material { get; set; }
}

public sealed class JournalEntry
{
    public int JournalEntryId { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int ReferenceId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Narration { get; set; } = string.Empty;
    public ICollection<JournalEntryLine> Lines { get; set; } = [];
}

public sealed class JournalEntryLine
{
    public int JournalLineId { get; set; }
    public int JournalEntryId { get; set; }
    public int AccountId { get; set; }
    public int? ProjectId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public JournalEntry? JournalEntry { get; set; }
    public ChartOfAccount? Account { get; set; }
    public Project? Project { get; set; }
}

public sealed class ProjectCostSummary
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal TotalEstimatedBudget { get; set; }
    public decimal ProductionDirectCost { get; set; }
    public decimal SiteDirectCost { get; set; }
    public decimal TotalDirectCost { get; set; }
}

public sealed class JournalEntryBalance
{
    public int JournalEntryId { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int ReferenceId { get; set; }
    public decimal? TotalDebit { get; set; }
    public decimal? TotalCredit { get; set; }
    public decimal? BalanceDifference { get; set; }
}
