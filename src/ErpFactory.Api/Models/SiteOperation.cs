using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ErpFactory.Api.Models;

public sealed class SiteOperation
{
    [Key]
    public int SiteOperationId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int ProjectItemId { get; set; }

    [Required]
    public DateTime OperationDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal InstalledQuantity { get; set; }


    public decimal SupervisorLaborCost { get; set; }


    public decimal DailyExpenses { get; set; }

    public Project? Project { get; set; }
    public ProjectItem? ProjectItem { get; set; }
    public ICollection<SiteMaterialConsumption> Consumption { get; set; } = new List<SiteMaterialConsumption>();
}
