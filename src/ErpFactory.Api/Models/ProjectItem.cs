namespace ErpFactory.Api.Models;

public sealed class ProjectItem : BaseEntity
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