namespace ErpFactory.Api.Models;

public sealed class Mold : BaseEntity
{
    public int MoldId { get; set; }
    public string MoldName { get; set; } = string.Empty;
    public decimal CostToBuild { get; set; }
    public int ExpectedLifespanUses { get; set; }
    public int CurrentUsesCount { get; set; }
    public string MoldStatus { get; set; } = "Available";
    public ICollection<ProjectMold> ProjectMolds { get; set; } = new List<ProjectMold>();
}