using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ErpFactory.Api.Services;

public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly Data.ErpFactoryDbContext _db;
    private readonly IPasswordHasher _hasher;

    public BasicAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        Data.ErpFactoryDbContext db,
        IPasswordHasher hasher)
        : base(options, logger, encoder, clock)
    {
        _db = db;
        _hasher = hasher;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // =========================
        // 1. Check Header
        // =========================
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization Header");

        var header = Request.Headers["Authorization"].ToString();

        if (!header.StartsWith("Basic "))
            return AuthenticateResult.Fail("Invalid Authorization Header");

        try
        {
            // =========================
            // 2. Decode Base64
            // =========================
            var encoded = header.Substring("Basic ".Length).Trim();
            var decodedBytes = Convert.FromBase64String(encoded);
            var decoded = Encoding.UTF8.GetString(decodedBytes);

            var parts = decoded.Split(':', 2);

            if (parts.Length != 2)
                return AuthenticateResult.Fail("Invalid Authorization Format");

            var username = parts[0];
            var password = parts[1];

            // =========================
            // 3. Get User + Role
            // =========================
            var user = await _db.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user is null)
                return AuthenticateResult.Fail("Invalid username or password");

            // =========================
            // 4. Verify Password
            // =========================
            if (!_hasher.Verify(user.PasswordHash, password))
                return AuthenticateResult.Fail("Invalid username or password");

            // =========================
            // 5. Create Claims (RBAC)
            // =========================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "Unknown")
            };

            // =========================
            // 6. Create Identity
            // =========================
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            // =========================
            // 7. Return Success
            // =========================
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }
}