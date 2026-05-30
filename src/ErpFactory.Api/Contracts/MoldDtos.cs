using System;
using System.Collections.Generic;

namespace ErpFactory.Api.Contracts;

public sealed class CreateMoldRequest
{
    public string MoldName { get; set; } = string.Empty;
    public decimal CostToBuild { get; set; }
    public int ExpectedLifespanUses { get; set; }
    public string? MoldStatus { get; set; }
}

public sealed class UpdateMoldStatusRequest
{
    public string MoldStatus { get; set; } = string.Empty;
}

public sealed class AllocateMoldRequest
{
    public int MoldId { get; set; }
    public int AllocQuantity { get; set; }
}
