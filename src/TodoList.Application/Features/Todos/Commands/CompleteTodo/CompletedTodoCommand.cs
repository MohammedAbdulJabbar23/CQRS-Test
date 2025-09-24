

using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Common.Exceptions;
using TodoList.Domain.Entities;

namespace TodoList.Application.Features.Todos.Commands.DeleteTodo;


public class CompletedTodoCommand : IRequest
{
    public Guid Id { get; set; }
}

public class CompletedTodoCommandHandler : IRequestHandler<CompletedTodoCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public CompletedTodoCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUser = currentUserService;
    }
    public async Task Handle(CompletedTodoCommand command, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;
        var todo = await _context.TodoItems
            .Where(c => c.Id == command.Id)
            .Where(c => c.UserId == currentUserId)
            .FirstOrDefaultAsync(ct);
        if (todo == null)
        {
            throw new NotFoundException(nameof(TodoItem), command.Id);
        }
        todo.Status = Domain.Enums.TodoStatus.Completed;
        todo.UpdatedAt = DateTime.UtcNow;
        todo.UpdatedBy = currentUserId;
        _context.TodoItems.Update(todo);
        await _context.SaveChangesAsync(ct);
    }
}