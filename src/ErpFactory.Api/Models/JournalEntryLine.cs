namespace ErpFactory.Api.Models;

public sealed class JournalEntryLine : BaseEntity
{
    public int JournalLineId { get; set; }
    public int JournalEntryId { get; set; }
    public int AccountId { get; set; }
    public int? ProjectId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public JournalEntry? JournalEntry { get; set; }
    public ChartOfAccount? Account { get; set; }
    public Project? Project { get; set; }
}