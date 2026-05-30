namespace ErpFactory.Api.Contracts;

public sealed class CreateCustomerRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int? AccountId { get; set; }
}