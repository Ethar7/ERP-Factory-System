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
public sealed class CustomersController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<Customer>>>> GetAll(CancellationToken ct)
    {
        var customers = await db.Customers.AsNoTracking().OrderBy(x => x.CustomerName).ToListAsync(ct);
        return OkCollection(customers);
    }

    [HttpGet("{customerId:int}", Name = nameof(GetCustomerById))]
    public async Task<ActionResult<ApiResponse<Customer>>> GetCustomerById(int customerId, CancellationToken ct)
    {
        var customer = await db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerId == customerId, ct);
        return customer is null ? NotFoundResponse<Customer>() : OkResponse(customer);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdResponse>>> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        var customer = new Customer
        {
            CustomerName = request.CustomerName,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            AccountId = request.AccountId
        };

        db.Customers.Add(customer);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ApiResponse<IdResponse>.Fail("Database update failed: " + ex.Message));
        }
        return CreatedResponse(nameof(GetCustomerById), new { customerId = customer.CustomerId }, new IdResponse(customer.CustomerId));
    }

    [HttpPut("{customerId:int}")]
    public async Task<ActionResult<ApiResponse<Customer>>> Update(int customerId, CreateCustomerRequest request, CancellationToken ct)
    {
        var customer = await db.Customers.FindAsync([customerId], ct);
        if (customer is null)
        {
            return NotFoundResponse<Customer>();
        }

        customer.CustomerName = request.CustomerName;
        customer.ContactPerson = request.ContactPerson;
        customer.Phone = request.Phone;
        customer.Email = request.Email;
        customer.Address = request.Address;
        customer.AccountId = request.AccountId;

        await db.SaveChangesAsync(ct);
        return OkResponse(customer);
    }

    [HttpGet("{customerId:int}/projects")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<Project>>>> GetProjects(int customerId, CancellationToken ct)
    {
        var projects = await db.Projects.AsNoTracking().Where(x => x.CustomerId == customerId).OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
        return OkCollection(projects);
    }
}
