using ErpFactory.Api.Contracts;
using ErpFactory.Api.Models;
using ErpFactory.Api.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly Data.ErpFactoryDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(
        Data.ErpFactoryDbContext db,
        IPasswordHasher hasher,
        IJwtTokenService jwtTokenService)
    {
        _db = db;
        _hasher = hasher;
        _jwtTokenService = jwtTokenService;
    }

    // =========================
    // REGISTER (NO ROLE INPUT)
    // =========================
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request"));

        if (await _db.Users.AnyAsync(u => u.Username == req.Username))
            return BadRequest(ApiResponse<object>.Fail("Username already exists"));

        Role role;

        // لو مفيش أي users -> أول واحد يبقى Admin
        if (!await _db.Users.AnyAsync())
        {
            role = await _db.Roles.FirstAsync(r => r.Name == "Admin");
        }
        else
        {
            // أي users بعد كده -> Viewer
            role = await _db.Roles.FirstAsync(r => r.Name == "Viewer");
        }

        if (role == null)
        {
            role = new Role { Name = "Viewer" };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
        }

        var user = new User
        {
            Username = req.Username,
            PasswordHash = _hasher.Hash(req.Password),
            FullName = req.FullName ?? string.Empty,
            Email = req.Email ?? string.Empty,
            RoleId = role.RoleId
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(
            new AuthUserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = role.Name
            },
            "User registered successfully"
        ));
    }

    // =========================
    // LOGIN
    // =========================
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request"));

        var user = await _db.Users
            .Include(u => u.Role)
            .SingleOrDefaultAsync(u => u.Username == req.Username);

        if (user == null || !_hasher.Verify(user.PasswordHash, req.Password))
            return Unauthorized(ApiResponse<object>.Fail("Invalid credentials"));

        var token = _jwtTokenService.CreateToken(user);

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

        return Ok(ApiResponse<LoginResponse>.Ok(response, "Login successful"));
    }
}
