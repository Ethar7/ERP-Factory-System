namespace ErpFactory.Api.Contracts;

public sealed class ProjectCostSummary
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal TotalEstimatedBudget { get; set; }
    public decimal ProductionDirectCost { get; set; }
    public decimal SiteDirectCost { get; set; }
    public decimal TotalDirectCost { get; set; }
}