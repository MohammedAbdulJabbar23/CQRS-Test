


using MediatR;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Events;

namespace TodoList.Application.Features.Todos.Commands.CreateTodo;


public class CreateTodoCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
}

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public CreateTodoCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IMediator mediator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate?.ToUniversalTime(),
            Status = TodoStatus.Pending,
            UserId = _currentUserService.UserId!,
            CreatedBy = _currentUserService.UserId!,
            CreatedAt = DateTime.UtcNow
        };
        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(
            new TodoItemCreatedEvent(todo.Id, todo.Title, todo.UserId),
            cancellationToken);
        return todo.Id;
    }
}
