using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpFactory.Api.Models;

public sealed class Project : BaseEntity
{
    [Key]
    public int ProjectId { get; set; }

    [Required]
    public string ProjectName { get; set; } = string.Empty;

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    [StringLength(50)]
    public string ProjectStatus { get; set; } = "Draft";

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal TotalEstimatedBudget { get; set; }


    public Customer? Customer { get; set; }

    public ICollection<ProjectItem> Items { get; set; } = new List<ProjectItem>();
    public ICollection<ProjectMold> ProjectMolds { get; set; } = new List<ProjectMold>();
    public ICollection<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();
    public ICollection<DeliveryOrder> DeliveryOrders { get; set; } = new List<DeliveryOrder>();
    public ICollection<SiteOperation> SiteOperations { get; set; } = new List<SiteOperation>();
}
