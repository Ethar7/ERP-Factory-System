namespace ErpFactory.Api.Contracts;

public sealed class CreateSiteOperationRequest
{
    public int ProjectId { get; set; }
    public int ProjectItemId { get; set; }
    public decimal InstalledQuantity { get; set; }
    public decimal SupervisorLaborCost { get; set; }
    public decimal DailyExpenses { get; set; }
}

public sealed class CreateSiteConsumptionRequest
{
    public int MaterialId { get; set; }
    public decimal QuantityConsumed { get; set; }
}
