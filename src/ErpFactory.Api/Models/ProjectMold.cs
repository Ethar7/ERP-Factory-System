namespace ErpFactory.Api.Models;

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