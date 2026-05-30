using ErpFactory.Api.Contracts;
using ErpFactory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly Data.ErpFactoryDbContext _db;

    public AdminUsersController(Data.ErpFactoryDbContext db)
    {
        _db = db;
    }

    // =========================
    // GET ALL USERS
    // =========================
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Users
            .Include(u => u.Role)
            .Select(u => new
            {
                u.UserId,
                u.Username,
                u.FullName,
                u.Email,
                Role = u.Role.Name
            })
            .ToListAsync();

        return Ok(users);
    }

    // =========================
    // ASSIGN / CHANGE ROLE
    // =========================
    [HttpPut("{userId}/role")]
    public async Task<IActionResult> ChangeRole(int userId, [FromBody] string roleName)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return NotFound("User not found");

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
            role = new Role { Name = roleName };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
        }

        user.RoleId = role.RoleId;

        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(
            new
            {
                user.UserId,
                Role = role.Name
            },
            "Role updated successfully"
        ));
    }
}