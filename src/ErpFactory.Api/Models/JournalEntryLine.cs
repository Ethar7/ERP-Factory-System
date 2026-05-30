using System.ComponentModel.DataAnnotations;

namespace ErpFactory.Api.Models;

public sealed class JournalEntryLine
{
    [Key]
    public int JournalLineId { get; set; }

    [Required]
    public int JournalEntryId { get; set; }

    [Required]
    public int AccountId { get; set; }

    public int? ProjectId { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public JournalEntry? JournalEntry { get; set; }
    public ChartOfAccount? Account { get; set; }
    public Project? Project { get; set; }
}
