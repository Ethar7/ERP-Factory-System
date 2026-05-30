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