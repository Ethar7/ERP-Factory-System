using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,ProjectManager")]
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
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ApiResponse<IdResponse>.Fail("Database update failed: " + ex.Message));
        }
        return CreatedResponse(nameof(GetDeliveryOrderById), new { deliveryOrderId = order.DeliveryOrderId }, new IdResponse(order.DeliveryOrderId));
    }

    [HttpPut("{deliveryOrderId:int}")]
    public async Task<ActionResult<ApiResponse<DeliveryOrder>>> Update(int deliveryOrderId, CreateDeliveryOrderRequest request, CancellationToken ct)
    {
        var order = await db.DeliveryOrders.FindAsync(new object[] { deliveryOrderId }, ct);
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
        var order = await db.DeliveryOrders.FindAsync(new object[] { deliveryOrderId }, ct);
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

    [HttpGet("{deliveryOrderId:int}/tracking")]
    public async Task<ActionResult<ApiResponse<object>>> Tracking(int deliveryOrderId, CancellationToken ct)
    {
        var order = await db.DeliveryOrders.AsNoTracking().Include(x => x.Items).FirstOrDefaultAsync(x => x.DeliveryOrderId == deliveryOrderId, ct);
        if (order is null) return NotFoundResponse<object>();

        return OkResponse<object>(new
        {
            order.DeliveryOrderId,
            order.DeliveryTicketNumber,
            order.LoadingTicketNumber,
            order.DriverName,
            order.VehicleNumber,
            order.DeliveryStatus,
            Items = order.Items.Select(i => new { i.DeliveryItemId, i.ProjectItemId, i.QuantityShipped, i.QuantityReceived, i.QuantityDamagedInTransit })
        });
    }

    [HttpPatch("{deliveryOrderId:int}/items/{deliveryItemId:int}/receive-confirmation")]
    public async Task<ActionResult<ApiResponse<DeliveryItem>>> ConfirmReceive(
        int deliveryOrderId,
        int deliveryItemId,
        ReceiveDeliveryItemRequest request,
        CancellationToken ct)
    {
        var item = await db.DeliveryItems.FirstOrDefaultAsync(x => x.DeliveryOrderId == deliveryOrderId && x.DeliveryItemId == deliveryItemId, ct);
        if (item is null)
        {
            return NotFoundResponse<DeliveryItem>();
        }

        item.QuantityReceived = request.QuantityReceived;
        item.QuantityDamagedInTransit = request.QuantityDamagedInTransit;
        await db.SaveChangesAsync(ct);
        return OkResponse(item);
    }
}
