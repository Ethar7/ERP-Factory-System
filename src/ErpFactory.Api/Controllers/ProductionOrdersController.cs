using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

[Route("api/v1/production-orders")]
public sealed class ProductionOrdersController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProductionOrder>>>> GetAll(
        [FromQuery] int? projectId,
        CancellationToken ct)
    {
        var query = db.ProductionOrders.AsNoTracking().AsQueryable();
        if (projectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == projectId.Value);
        }

        return OkCollection(await query.OrderByDescending(x => x.OrderDate).ToListAsync(ct));
    }

    [HttpGet("{productionOrderId:int}", Name = nameof(GetProductionOrderById))]
    public async Task<ActionResult<ApiResponse<ProductionOrder>>> GetProductionOrderById(int productionOrderId, CancellationToken ct)
    {
        var order = await db.ProductionOrders
            .AsNoTracking()
            .Include(x => x.MaterialConsumption)
            .FirstOrDefaultAsync(x => x.ProductionOrderId == productionOrderId, ct);

        return order is null ? NotFoundResponse<ProductionOrder>() : OkResponse(order);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Create(CreateProductionOrderRequest request, CancellationToken ct)
    {
        var order = new ProductionOrder
        {
            ProjectId = request.ProjectId,
            ProjectItemId = request.ProjectItemId,
            MixDesignId = request.MixDesignId,
            MoldId = request.MoldId,
            BatchNumber = request.BatchNumber,
            TargetQuantity = request.TargetQuantity,
            LaborCost = request.LaborCost,
            MoldDepreciationCost = request.MoldDepreciationCost
        };

        db.ProductionOrders.Add(order);
        await db.SaveChangesAsync(ct);
        return CreatedResponse(nameof(GetProductionOrderById), new { productionOrderId = order.ProductionOrderId }, new IdResponse(order.ProductionOrderId));
    }

    [HttpPut("{productionOrderId:int}")]
    public async Task<ActionResult<ApiResponse<ProductionOrder>>> Update(int productionOrderId, CreateProductionOrderRequest request, CancellationToken ct)
    {
        var order = await db.ProductionOrders.FindAsync([productionOrderId], ct);
        if (order is null)
        {
            return NotFoundResponse<ProductionOrder>();
        }

        order.ProjectId = request.ProjectId;
        order.ProjectItemId = request.ProjectItemId;
        order.MixDesignId = request.MixDesignId;
        order.MoldId = request.MoldId;
        order.BatchNumber = request.BatchNumber;
        order.TargetQuantity = request.TargetQuantity;
        order.LaborCost = request.LaborCost;
        order.MoldDepreciationCost = request.MoldDepreciationCost;

        await db.SaveChangesAsync(ct);
        return OkResponse(order);
    }

    [HttpPatch("{productionOrderId:int}/status")]
    public async Task<ActionResult<ApiResponse<ProductionOrder>>> UpdateStatus(int productionOrderId, UpdateProductionStatusRequest request, CancellationToken ct)
    {
        var order = await db.ProductionOrders.FindAsync([productionOrderId], ct);
        if (order is null)
        {
            return NotFoundResponse<ProductionOrder>();
        }

        order.ProductionStatus = request.ProductionStatus;
        await db.SaveChangesAsync(ct);
        return OkResponse(order);
    }

    [HttpPatch("{productionOrderId:int}/quality-check")]
    public async Task<ActionResult<ApiResponse<ProductionOrder>>> RecordQualityCheck(int productionOrderId, RecordQualityCheckRequest request, CancellationToken ct)
    {
        var order = await db.ProductionOrders.FindAsync([productionOrderId], ct);
        if (order is null)
        {
            return NotFoundResponse<ProductionOrder>();
        }

        order.ProducedQuantity = request.ProducedQuantity;
        order.GoodQuantity = request.GoodQuantity;
        order.RejectedQuantity = request.RejectedQuantity;
        order.LaborCost = request.LaborCost;
        order.MoldDepreciationCost = request.MoldDepreciationCost;
        order.ProductionStatus = "QualityCheck";

        await db.SaveChangesAsync(ct);
        return OkResponse(order);
    }

    [HttpGet("{productionOrderId:int}/material-consumption")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProductionMaterialConsumption>>>> GetConsumption(int productionOrderId, CancellationToken ct) =>
        OkCollection(await db.ProductionMaterialConsumption.AsNoTracking().Where(x => x.ProductionOrderId == productionOrderId).ToListAsync(ct));

    [HttpPost("{productionOrderId:int}/material-consumption")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> RecordConsumption(int productionOrderId, RecordProductionConsumptionRequest request, CancellationToken ct)
    {
        var consumption = new ProductionMaterialConsumption
        {
            ProductionOrderId = productionOrderId,
            MaterialId = request.MaterialId,
            ActualQtyConsumed = request.ActualQtyConsumed,
            StandardQtyExpected = request.StandardQtyExpected
        };

        db.ProductionMaterialConsumption.Add(consumption);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(consumption.ConsumptionId), "Resource created successfully");
    }

    [HttpPost("{productionOrderId:int}/post-accounting")]
    public async Task<ActionResult<ApiResponse<ProductionOrder>>> MarkAccountingPosted(int productionOrderId, CancellationToken ct)
    {
        var order = await db.ProductionOrders.FindAsync([productionOrderId], ct);
        if (order is null)
        {
            return NotFoundResponse<ProductionOrder>();
        }

        order.IsAccountingPosted = true;
        await db.SaveChangesAsync(ct);
        return OkResponse(order, "Accounting posting flag updated");
    }
}
