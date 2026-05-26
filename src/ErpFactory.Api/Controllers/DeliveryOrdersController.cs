using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

[Route("api/v1/delivery-orders")]
public sealed class DeliveryOrdersController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<DeliveryOrder>>>> GetAll([FromQuery] int? projectId, CancellationToken ct)
    {
        var query = db.DeliveryOrders.AsNoTracking().AsQueryable();
        if (projectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == projectId.Value);
        }

        return OkCollection(await query.OrderByDescending(x => x.DeliveryDate).ToListAsync(ct));
    }

    [HttpGet("{deliveryOrderId:int}", Name = nameof(GetDeliveryOrderById))]
    public async Task<ActionResult<ApiResponse<DeliveryOrder>>> GetDeliveryOrderById(int deliveryOrderId, CancellationToken ct)
    {
        var order = await db.DeliveryOrders.AsNoTracking().Include(x => x.Items).FirstOrDefaultAsync(x => x.DeliveryOrderId == deliveryOrderId, ct);
        return order is null ? NotFoundResponse<DeliveryOrder>() : OkResponse(order);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Create(CreateDeliveryOrderRequest request, CancellationToken ct)
    {
        var order = new DeliveryOrder
        {
            ProjectId = request.ProjectId,
            DriverName = request.DriverName,
            VehicleNumber = request.VehicleNumber,
            LoadingTicketNumber = request.LoadingTicketNumber,
            DeliveryTicketNumber = request.DeliveryTicketNumber
        };

        db.DeliveryOrders.Add(order);
        await db.SaveChangesAsync(ct);
        return CreatedResponse(nameof(GetDeliveryOrderById), new { deliveryOrderId = order.DeliveryOrderId }, new IdResponse(order.DeliveryOrderId));
    }

    [HttpPut("{deliveryOrderId:int}")]
    public async Task<ActionResult<ApiResponse<DeliveryOrder>>> Update(int deliveryOrderId, CreateDeliveryOrderRequest request, CancellationToken ct)
    {
        var order = await db.DeliveryOrders.FindAsync([deliveryOrderId], ct);
        if (order is null)
        {
            return NotFoundResponse<DeliveryOrder>();
        }

        order.ProjectId = request.ProjectId;
        order.DriverName = request.DriverName;
        order.VehicleNumber = request.VehicleNumber;
        order.LoadingTicketNumber = request.LoadingTicketNumber;
        order.DeliveryTicketNumber = request.DeliveryTicketNumber;
        await db.SaveChangesAsync(ct);
        return OkResponse(order);
    }

    [HttpPatch("{deliveryOrderId:int}/status")]
    public async Task<ActionResult<ApiResponse<DeliveryOrder>>> UpdateStatus(int deliveryOrderId, UpdateDeliveryStatusRequest request, CancellationToken ct)
    {
        var order = await db.DeliveryOrders.FindAsync([deliveryOrderId], ct);
        if (order is null)
        {
            return NotFoundResponse<DeliveryOrder>();
        }

        order.DeliveryStatus = request.DeliveryStatus;
        await db.SaveChangesAsync(ct);
        return OkResponse(order);
    }

    [HttpPost("{deliveryOrderId:int}/items")]
    public async Task<ActionResult<ApiResponse<IdResponse>>> AddItem(int deliveryOrderId, CreateDeliveryItemRequest request, CancellationToken ct)
    {
        var item = new DeliveryItem
        {
            DeliveryOrderId = deliveryOrderId,
            ProjectItemId = request.ProjectItemId,
            QuantityShipped = request.QuantityShipped,
            QuantityReceived = request.QuantityReceived,
            QuantityDamagedInTransit = request.QuantityDamagedInTransit
        };

        db.DeliveryItems.Add(item);
        await db.SaveChangesAsync(ct);
        return OkResponse(new IdResponse(item.DeliveryItemId), "Resource created successfully");
    }
}
