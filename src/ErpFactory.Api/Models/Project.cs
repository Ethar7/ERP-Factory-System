namespace ErpFactory.Api.Models;

public sealed class Project : BaseEntity
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ProjectStatus { get; set; } = "Draft";
    public decimal TotalEstimatedBudget { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<ProjectItem> Items { get; set; } = new List<ProjectItem>();
    public ICollection<ProjectMold> ProjectMolds { get; set; } = new List<ProjectMold>();
    public ICollection<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();
    public ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();
    public ICollection<SiteOperation> SiteOperations { get; set; } = new List<SiteOperation>();
}