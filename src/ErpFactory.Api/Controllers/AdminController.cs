using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public sealed class AdminUsersController(ErpFactoryDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<object>>>> GetUsers(CancellationToken ct)
    {
        var users = await db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Select(u => new
            {
                u.UserId,
                u.Username,
                u.FullName,
                u.Email,
                Role = u.Role.Name
            })
            .ToListAsync(ct);

        // تحويل القائمة إلى object صريح لتوافق الـ ApiResponse
        return OkCollection<object>(users.Cast<object>().ToList());
    }

    [HttpPut("{userId:int}/role")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeRole(int userId, [FromBody] string roleName, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);

        if (user is null)
        {
            return NotFoundResponse<object>("User not found");
        }

        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);

        if (role is null)
        {
            role = new Role { Name = roleName };
            db.Roles.Add(role);
            await db.SaveChangesAsync(ct);
        }

        user.RoleId = role.RoleId;
        await db.SaveChangesAsync(ct);

        // تعريف النتيجة كـ object لضمان توافق النوع مع الـ Generic ApiResponse
        object result = new
        {
            user.UserId,
            Role = role.Name
        };

        return OkResponse<object>(result, "Role updated successfully");
    }
}