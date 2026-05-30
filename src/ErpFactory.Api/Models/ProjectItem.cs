using System.ComponentModel.DataAnnotations;

namespace ErpFactory.Api.Models;

public sealed class ProjectItem
{
    [Key]
    public int ProjectItemId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    [StringLength(50)]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string ItemName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal RequiredQuantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal EstimatedUnitPrice { get; set; }

    [Range(0, 100)]
    public decimal TaxRate { get; set; }

    public decimal TaxAmount { get; private set; }

    public decimal TotalPrice { get; private set; }

    public decimal TotalPriceWithTax { get; private set; }

    public Project? Project { get; set; }
}
