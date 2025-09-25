using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoList.Application.Common.Exceptions;
using TodoList.Application.Common.Interfaces;
using TodoList.Application.Features.Todos.Commands.DeleteTodo;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Persistence;

namespace TodoList.UnitTests.Features.Todos;

public class CompleteTodoCommandTests : IDisposable
{
    private readonly IApplicationDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly Mock<IMediator> _mediator;
    private readonly CompletedTodoCommandHandler _handler;
    private readonly string _userId = "test-user-id";

    public CompleteTodoCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _currentUserService = new Mock<ICurrentUserService>();
        _mediator = new Mock<IMediator>();

        _currentUserService.Setup(x => x.UserId).Returns(_userId);

        _handler = new CompletedTodoCommandHandler(_context, _currentUserService.Object);
    }

    [Fact]
    public async Task Should_Complete_Todo()
    {
        // Arrange
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Todo",
            Status = Domain.Enums.TodoStatus.Pending,
            UserId = _userId,
            Priority = Domain.Enums.TodoPriority.Critical
        };

        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = new CompletedTodoCommand { Id = todo.Id };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedTodo = await _context.TodoItems.FindAsync(todo.Id);
        Assert.Equivalent(updatedTodo!.Status, Domain.Enums.TodoStatus.Completed);
    }
    [Fact]
    public async Task Handle_UserNotOwner_ThrowsForbiddenException()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var todo = new TodoItem
        {
            Id = todoId,
            Title = "Other User's Todo",
            Priority = Domain.Enums.TodoPriority.Medium,
            Status = Domain.Enums.TodoStatus.Pending,
            UserId = "other-user-id",
            CreatedBy = "other-user-id",
            CreatedAt = DateTime.UtcNow
        };

        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Current user is not the owner
        _currentUserService.Setup(x => x.UserId).Returns("test-user-id");

        var command = new CompletedTodoCommand { Id = todoId };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    public void Dispose()
    {

    }
}
