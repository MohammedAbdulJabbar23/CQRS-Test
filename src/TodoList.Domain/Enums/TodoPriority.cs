using System.ComponentModel;

namespace TodoList.Domain.Enums;

public enum TodoPriority
{
    [Description("Low priority task")]
    Low = 0,

    [Description("Medium priority task")]
    Medium = 1,

    [Description("High priority task")]
    High = 2,

    [Description("Critical priority task")]
    Critical = 3
}

