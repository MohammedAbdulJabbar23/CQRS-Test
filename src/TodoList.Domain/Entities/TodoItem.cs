

using TodoList.Domain.Common;
using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public class TodoItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; }
    public TodoStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public User User { get; set; } = null!;
}


