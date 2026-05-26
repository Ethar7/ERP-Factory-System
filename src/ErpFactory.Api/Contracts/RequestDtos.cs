namespace ErpFactory.Api.Contracts;

public sealed record CreateCustomerRequest(
    string CustomerName,
    string? ContactPerson,
    string? Phone,
    string? Email,
    string? Address,
    int? AccountId);

public sealed record CreateProjectItemRequest(
    string ItemCode,
    string ItemName,
    string Unit,
    decimal RequiredQuantity,
    decimal EstimatedUnitPrice,
    decimal TaxRate);

public sealed record CreateProjectRequest(
    string ProjectName,
    int CustomerId,
    DateTime? StartDate,
    decimal TotalEstimatedBudget,
    IReadOnlyCollection<CreateProjectItemRequest>? Items);

public sealed record UpdateProjectStatusRequest(string ProjectStatus);

public sealed record CreateMoldRequest(
    string MoldName,
    decimal CostToBuild,
    int ExpectedLifespanUses,
    string? MoldStatus);

public sealed record UpdateMoldStatusRequest(string MoldStatus);

public sealed record AllocateMoldRequest(int MoldId, int AllocQuantity);

public sealed record CreateMixIngredientRequest(int RawMaterialId, decimal StandardQtyPerUnit);

public sealed record CreateMixDesignRequest(
    string MixName,
    string? TargetStrength,
    decimal StandardCostPerUnit,
    IReadOnlyCollection<CreateMixIngredientRequest>? Ingredients);

public sealed record CreateInventoryItemRequest(
    string ItemName,
    string ItemType,
    string Unit,
    decimal CurrentStock,
    decimal AverageCost);

public sealed record CreateInventoryTransactionRequest(
    int ItemId,
    int? ProjectId,
    string TransactionType,
    decimal Quantity,
    decimal UnitCost,
    string? ReferenceType,
    int? ReferenceId,
    string? Notes);

public sealed record CreateProductionOrderRequest(
    int ProjectId,
    int ProjectItemId,
    int MixDesignId,
    int MoldId,
    string BatchNumber,
    decimal TargetQuantity,
    decimal LaborCost,
    decimal MoldDepreciationCost);

public sealed record UpdateProductionStatusRequest(string ProductionStatus);

public sealed record RecordQualityCheckRequest(
    decimal ProducedQuantity,
    decimal GoodQuantity,
    decimal RejectedQuantity,
    decimal LaborCost,
    decimal MoldDepreciationCost);

public sealed record RecordProductionConsumptionRequest(
    int MaterialId,
    decimal ActualQtyConsumed,
    decimal StandardQtyExpected);

public sealed record CreateDeliveryOrderRequest(
    int ProjectId,
    string? DriverName,
    string? VehicleNumber,
    string? LoadingTicketNumber,
    string? DeliveryTicketNumber);

public sealed record UpdateDeliveryStatusRequest(string DeliveryStatus);

public sealed record CreateDeliveryItemRequest(
    int ProjectItemId,
    decimal QuantityShipped,
    decimal? QuantityReceived,
    decimal QuantityDamagedInTransit);

public sealed record ReceiveDeliveryItemRequest(decimal QuantityReceived, decimal QuantityDamagedInTransit);

public sealed record CreateSiteOperationRequest(
    int ProjectId,
    int ProjectItemId,
    decimal InstalledQuantity,
    decimal SupervisorLaborCost,
    decimal DailyExpenses);

public sealed record CreateSiteConsumptionRequest(int MaterialId, decimal QuantityConsumed);

public sealed record CreateAccountRequest(string AccountCode, string AccountName, string AccountType);

public sealed record CreateJournalLineRequest(int AccountId, int? ProjectId, decimal Debit, decimal Credit);

public sealed record CreateJournalEntryRequest(
    string ReferenceType,
    int ReferenceId,
    string Narration,
    IReadOnlyCollection<CreateJournalLineRequest>? Lines);
