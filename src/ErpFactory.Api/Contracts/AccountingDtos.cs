namespace ErpFactory.Api.Contracts;

public sealed class CreateAccountRequest
{
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
}

public sealed class CreateJournalEntryRequest
{
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? Narration { get; set; }


    public IReadOnlyCollection<CreateJournalLineRequest> Lines { get; set; } = Array.Empty<CreateJournalLineRequest>();
}

public sealed class CreateJournalLineRequest
{
    public int AccountId { get; set; }
    public int? ProjectId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}