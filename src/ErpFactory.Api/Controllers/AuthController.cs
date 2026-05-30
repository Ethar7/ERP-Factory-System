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
        if (await db.Users.AnyAsync(u => u.Username == req.Username, ct))
        {
            return FailResponse<AuthUserDto>("Username already exists");
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
            Username = req.Username,
            PasswordHash = hasher.Hash(req.Password),
            FullName = req.FullName ?? string.Empty,
            Email = req.Email ?? string.Empty,
            RoleId = role.RoleId
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

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
        var user = await db.Users
            .Include(u => u.Role)
            .SingleOrDefaultAsync(u => u.Username == req.Username, ct);

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