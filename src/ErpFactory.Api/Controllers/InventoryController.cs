using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

[Route("api/v1/inventory")]
public sealed class InventoryController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet("items")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<InventoryItem>>>> GetItems(CancellationToken ct) =>
        OkCollection(await db.InventoryItems.AsNoTracking().OrderBy(x => x.ItemName).ToListAsync(ct));

    [HttpGet("items/{itemId:int}", Name = nameof(GetInventoryItemById))]
    public async Task<ActionResult<ApiResponse<InventoryItem>>> GetInventoryItemById(int itemId, CancellationToken ct)
    {
        var item = await db.InventoryItems.AsNoTracking().FirstOrDefaultAsync(x => x.ItemId == itemId, ct);
        return item is null ? NotFoundResponse<InventoryItem>() : OkResponse(item);
    }

    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> CreateItem(CreateInventoryItemRequest request, CancellationToken ct)
    {
        var item = new InventoryItem
        {
            ItemName = request.ItemName,
            ItemType = request.ItemType,
            Unit = request.Unit,
            CurrentStock = request.CurrentStock,
            AverageCost = request.AverageCost
        };

        db.InventoryItems.Add(item);
        await db.SaveChangesAsync(ct);
        return CreatedResponse(nameof(GetInventoryItemById), new { itemId = item.ItemId }, new IdResponse(item.ItemId));
    }

    [HttpPut("items/{itemId:int}")]
    public async Task<ActionResult<ApiResponse<InventoryItem>>> UpdateItem(int itemId, CreateInventoryItemRequest request, CancellationToken ct)
    {
        var item = await db.InventoryItems.FindAsync([itemId], ct);
        if (item is null)
        {
            return NotFoundResponse<InventoryItem>();
        }

        item.ItemName = request.ItemName;
        item.ItemType = request.ItemType;
        item.Unit = request.Unit;
        item.CurrentStock = request.CurrentStock;
        item.AverageCost = request.AverageCost;

        await db.SaveChangesAsync(ct);
        return OkResponse(item);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<InventoryTransaction>>>> GetTransactions(
        [FromQuery] int? itemId,
        [FromQuery] int? projectId,
        CancellationToken ct)
    {
        var query = db.InventoryTransactions.AsNoTracking().Include(x => x.Item).AsQueryable();

        if (itemId.HasValue)
        {
            query = query.Where(x => x.ItemId == itemId.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == projectId.Value);
        }

        return OkCollection(await query.OrderByDescending(x => x.TransactionDate).ToListAsync(ct));
    }

    [HttpPost("transactions")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> CreateTransaction(CreateInventoryTransactionRequest request, CancellationToken ct)
    {
        var transaction = new InventoryTransaction
        {
            ItemId = request.ItemId,
            ProjectId = request.ProjectId,
            TransactionType = request.TransactionType,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            ReferenceType = request.ReferenceType,
            ReferenceId = request.ReferenceId,
            Notes = request.Notes
        };

        db.InventoryTransactions.Add(transaction);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(transaction.TransactionId), "Resource created successfully");
    }
}
