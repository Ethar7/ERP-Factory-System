using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpFactory.Api.DTOS;
using Microsoft.AspNetCore.Authorization;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Accountant")]
public sealed class AccountingController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet("chart-of-accounts")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ChartOfAccount>>>> GetAccounts(CancellationToken ct) =>
        OkCollection(await db.ChartOfAccounts.AsNoTracking().OrderBy(x => x.AccountCode).ToListAsync(ct));

    [HttpPost("chart-of-accounts")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> CreateAccount(CreateAccountRequest request, CancellationToken ct)
    {
        var account = new ChartOfAccount
        {
            AccountCode = request.AccountCode,
            AccountName = request.AccountName,
            AccountType = request.AccountType
        };

        db.ChartOfAccounts.Add(account);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(account.AccountId), "Resource created successfully");
    }

    [HttpGet("journal-entries")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<JournalEntry>>>> GetJournalEntries(CancellationToken ct) =>
        OkCollection(await db.JournalEntries.AsNoTracking().OrderByDescending(x => x.TransactionDate).ToListAsync(ct));

    [HttpGet("journal-entries/{journalEntryId:int}", Name = nameof(GetJournalEntryById))]
    public async Task<ActionResult<ApiResponse<JournalEntry>>> GetJournalEntryById(int journalEntryId, CancellationToken ct)
    {
        var entry = await db.JournalEntries.AsNoTracking().Include(x => x.Lines).FirstOrDefaultAsync(x => x.JournalEntryId == journalEntryId, ct);
        return entry is null ? NotFoundResponse<JournalEntry>() : OkResponse(entry);
    }

    [HttpPost("journal-entries")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> CreateJournalEntry(CreateJournalEntryRequest request, CancellationToken ct)
    {
        var entry = new JournalEntry
        {
            ReferenceType = request.ReferenceType ?? string.Empty,
            ReferenceId = request.ReferenceId,
            Narration = request.Narration ?? string.Empty,
            Lines = request.Lines?.Select(x => new JournalEntryLine
            {
                AccountId = x.AccountId,
                ProjectId = x.ProjectId,
                Debit = x.Debit,
                Credit = x.Credit
            }).ToList() ?? new List<JournalEntryLine>()
        };

        db.JournalEntries.Add(entry);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ApiResponse<IdResponse>.Fail("Database update failed: " + ex.Message));
        }
        return CreatedResponse(nameof(GetJournalEntryById), new { journalEntryId = entry.JournalEntryId }, new IdResponse(entry.JournalEntryId));
    }
}
