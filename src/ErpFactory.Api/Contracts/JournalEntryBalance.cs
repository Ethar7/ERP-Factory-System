namespace ErpFactory.Api.Contracts;

public sealed class JournalEntryBalance
{
    public int JournalEntryId { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal BalanceDifference { get; set; }
}