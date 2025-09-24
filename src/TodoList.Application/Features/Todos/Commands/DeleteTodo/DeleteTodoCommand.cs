

using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.Common.Exceptions;
using TodoList.Domain.Entities;

namespace TodoList.Application.Features.Todos.Commands.DeleteTodo;


public class DeleteTodoCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public DeleteTodoCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUser = currentUserService;
    }
    public async Task Handle(DeleteTodoCommand command, CancellationToken ct)
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
        todo.IsDeleted = true;
        todo.DeletedAt = DateTime.UtcNow;
        todo.DeletedBy = _currentUser.UserId;
        _context.TodoItems.Update(todo);
        await _context.SaveChangesAsync(ct);
    }
}