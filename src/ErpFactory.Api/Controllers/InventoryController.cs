using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,InventoryUser")]
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
        var item = await db.InventoryItems.FirstOrDefaultAsync(x => x.ItemId == itemId, ct);
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

    [HttpGet("items/{itemId:int}/balance")]
    public async Task<ActionResult<ApiResponse<object>>> GetItemBalance(int itemId, CancellationToken ct)
    {
        var item = await db.InventoryItems.AsNoTracking().FirstOrDefaultAsync(x => x.ItemId == itemId, ct);
        return item is null ? NotFoundResponse<object>() : OkResponse<object>(new { item.ItemId, item.ItemName, Balance = item.CurrentStock });
    }

    [HttpGet("items/{itemId:int}/transactions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<InventoryTransaction>>>> GetItemTransactions(int itemId, CancellationToken ct)
    {
        var tx = await db.InventoryTransactions.AsNoTracking().Where(x => x.ItemId == itemId).OrderByDescending(x => x.TransactionDate).ToListAsync(ct);
        return OkCollection(tx);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<InventoryItem>>>> LowStock(CancellationToken ct, [FromQuery] decimal threshold = 5m)
    {
        var items = await db.InventoryItems.AsNoTracking().Where(x => x.CurrentStock <= threshold).OrderBy(x => x.ItemName).ToListAsync(ct);
        return OkCollection(items);
    }
}