namespace ErpFactory.Api.Models;

public sealed class Customer : BaseEntity
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int? AccountId { get; set; }
    public ChartOfAccount? Account { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}