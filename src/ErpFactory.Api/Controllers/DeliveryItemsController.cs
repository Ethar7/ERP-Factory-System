using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ErpFactory.Api.Controllers;

[Route("api/v1/delivery-items")]
public sealed class DeliveryItemsController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpPut("{deliveryItemId:int}")]
    public async Task<ActionResult<ApiResponse<DeliveryItem>>> Update(int deliveryItemId, CreateDeliveryItemRequest request, CancellationToken ct)
    {
        var item = await db.DeliveryItems.FindAsync([deliveryItemId], ct);
        if (item is null)
        {
            return NotFoundResponse<DeliveryItem>();
        }

        item.ProjectItemId = request.ProjectItemId;
        item.QuantityShipped = request.QuantityShipped;
        item.QuantityReceived = request.QuantityReceived;
        item.QuantityDamagedInTransit = request.QuantityDamagedInTransit;
        await db.SaveChangesAsync(ct);
        return OkResponse(item);
    }

    [HttpPatch("{deliveryItemId:int}/receive-confirmation")]
    public async Task<ActionResult<ApiResponse<DeliveryItem>>> ConfirmReceive(int deliveryItemId, ReceiveDeliveryItemRequest request, CancellationToken ct)
    {
        var item = await db.DeliveryItems.FindAsync([deliveryItemId], ct);
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
