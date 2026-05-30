using System.Collections.Generic;

namespace ErpFactory.Api.Models;

public sealed class JournalEntry
{
    public int JournalEntryId { get; set; }
    public string ReferenceType { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Narration { get; set; } = string.Empty;
    public bool IsPosted { get; set; }
    public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
}
