using System.ComponentModel.DataAnnotations;

namespace ErpFactory.Api.Contracts;

public sealed class RegisterRequest
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string? FullName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? Role { get; set; }
}

public sealed class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed class AuthUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public AuthUserDto User { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAtUtc { get; set; }
}
