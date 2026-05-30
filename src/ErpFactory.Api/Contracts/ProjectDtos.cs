using System;
using System.Collections.Generic;

namespace ErpFactory.Api.Contracts;

public sealed class CreateProjectItemRequest
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal EstimatedUnitPrice { get; set; }
    public decimal TaxRate { get; set; }
}

public sealed class CreateProjectRequest
{
    public string ProjectName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime? StartDate { get; set; }
    public decimal TotalEstimatedBudget { get; set; }

    public IReadOnlyCollection<CreateProjectItemRequest>? Items { get; set; }
}

public sealed class UpdateProjectStatusRequest
{
    public string ProjectStatus { get; set; } = string.Empty;
}
