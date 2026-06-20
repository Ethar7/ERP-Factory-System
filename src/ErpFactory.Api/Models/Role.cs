namespace ErpFactory.Api.Models;

public sealed class Role : BaseEntity
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();

}