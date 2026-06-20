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

    [Authorize(Roles = "Admin")]
    [HttpPut("{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] string newRole)
    {
        // التأكد من أن الدور المرسل هو واحد من الأدوار المعتمدة
        var allowedRoles = new List<string> { "Admin", "ProjectManager", "InventoryUser", "Accountant" };
        
        if (!allowedRoles.Contains(newRole))
        {
            return BadRequest("صلاحية غير صالحة");
        }

        var user = await db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == newRole);
        if (role == null)
        {
            return BadRequest("الصلاحية المطلوبة غير موجودة في قاعدة البيانات");
        }

        user.RoleId = role.RoleId; // تحديث معرف الدور في قاعدة البيانات
        await db.SaveChangesAsync();
        
        return Ok(new { message = "تم تحديث الصلاحية بنجاح" });
    }

}