using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Common.Exceptions;
using TodoList.Application.Common.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.Features.Todos.Commands.UpdateTodo;

public class UpdateTodoCommand : IRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; }

    public TodoStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
}
public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateTodoCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateTodoCommand command, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var todo = await _context.TodoItems
            .Where(t => t.Id == command.Id)
            .Where(t => t.UserId == currentUserId)
            .FirstOrDefaultAsync(ct);

        if (todo == null)
        {
            throw new NotFoundException(nameof(TodoItem), command.Id);
        }

        todo.Title = command.Title;
        todo.Description = command.Description;
        todo.Priority = command.Priority;
        todo.Status = command.Status;
        todo.DueDate = command.DueDate;
        todo.UpdatedAt = DateTime.UtcNow;
        todo.UpdatedBy = currentUserId;

        _context.TodoItems.Update(todo);
        await _context.SaveChangesAsync(ct);
    }
}
