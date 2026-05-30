using System.Collections.Generic;

namespace ErpFactory.Api.Models;

public sealed class ChartOfAccount
{
    public int AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
