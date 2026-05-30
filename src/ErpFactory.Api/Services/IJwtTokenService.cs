using ErpFactory.Api.Models;

namespace ErpFactory.Api.Services;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(User user);
}

public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);