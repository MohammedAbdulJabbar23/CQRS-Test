using Microsoft.AspNetCore.Identity;

namespace TodoList.Domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
