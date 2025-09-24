using TodoList.Domain.Common;

namespace TodoList.Domain.Events;

public class TodoItemCreatedEvent : DomainEvent
{
    public Guid TodoItemId { get; }
    public string Title { get; }
    public string UserId { get; }

    public TodoItemCreatedEvent(Guid todoItemId, string title, string userId)
    {
        TodoItemId = todoItemId;
        Title = title;
        UserId = userId;
    }
}