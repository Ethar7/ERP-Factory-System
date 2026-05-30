namespace ErpFactory.Api.Models;

public sealed class SiteOperation : BaseEntity
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
    public ICollection<SiteMaterialConsumption> Consumption { get; set; } = new List<SiteMaterialConsumption>();
}