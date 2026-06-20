using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpFactory.Api.Contracts;
using ErpFactory.Api.Data;
using ErpFactory.Api.Models;
using ErpFactory.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(
    ErpFactoryDbContext db,
    IPasswordHasher hasher,
    IJwtTokenService jwtTokenService) : ApiControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthUserDto>>> Register(RegisterRequest req, CancellationToken ct)
    {
        var username = req.Username.Trim();

        if (await db.Users.AnyAsync(u => u.Username == username, ct))
        {
            return FailResponse<AuthUserDto>("Username already exists");
        }
        var userCount = await db.Users.CountAsync(ct);
    
        if (userCount >= 4)
        {
            return BadRequest("لا يمكن تسجيل مستخدمين جدد، تم الوصول للحد الأقصى (4 مستخدمين).");
        }
            string targetRoleName = !await db.Users.AnyAsync(ct) ? "Admin" : "Viewer";
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == targetRoleName, ct);

        if (role is null)
        {
            role = new Role { Name = targetRoleName };
            db.Roles.Add(role);
            await db.SaveChangesAsync(ct);
        }

        var user = new User
        {
            Username = username,
            PasswordHash = hasher.Hash(req.Password),
            FullName = req.FullName ?? string.Empty,
            Email = req.Email ?? string.Empty,
            RoleId = role.RoleId
        };

        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return FailResponse<AuthUserDto>("Username already exists");
        }

        var result = new AuthUserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = role.Name
        };

        return OkResponse(result, "User registered successfully");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest req, CancellationToken ct)
    {
        var username = req.Username.Trim();

        var user = await db.Users
            .Include(u => u.Role)
            .Where(u => u.Username == username)
            .OrderBy(u => u.UserId)
            .FirstOrDefaultAsync(ct);

        if (user is null || !hasher.Verify(user.PasswordHash, req.Password))
        {
            return FailResponse<LoginResponse>("Invalid credentials");
        }

        var token = jwtTokenService.CreateToken(user);

        var response = new LoginResponse
        {
            User = new AuthUserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name
            },
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc
        };

        return OkResponse(response, "Login successful");
    }
}

