using System.ComponentModel;

namespace TodoList.Domain.Enums;

public enum TodoStatus
{
    [Description("Task is pending and not started")]
    Pending = 0,
    
    [Description("Task is currently in progress")]
    InProgress = 1,
    
    [Description("Task has been completed")]
    Completed = 2,
    
    [Description("Task has been cancelled")]
    Cancelled = 3
}