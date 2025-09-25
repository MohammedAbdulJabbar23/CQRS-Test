using Microsoft.EntityFrameworkCore;
using TodoList.Api.COntrollers.V1;
using TodoList.Application.Features.Todos.Commands.CreateTodo;
using TodoList.Application.Features.Todos.Commands.UpdateTodo;
using TodoList.Application.Features.Todos.Queries.GetTodos;
using TodoList.Application.Features.Todos.Commands.DeleteTodo;
using TodoList.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TodoList.Tests.Integration;

public class TodoControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TodoController _controller;

    public TodoControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TodoController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateTodo_ShouldReturnGuid()
    {
        // Arrange
        var command = new CreateTodoCommand
        {
            Title = "Test Todo",
            Description = "Test Description",
            Priority = Domain.Enums.TodoPriority.Medium
        };

        var todoId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(todoId);

        // Act
        var result = await _controller.CreateTodo(command, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.Equal(todoId, result.Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnPagedResult()
    {
        // Arrange
        var query = new GetTodosQuery { PageNumber = 1, PageSize = 10 };
        var pagedResult = new PagedResult<TodoDto>
        {
            Items = new List<TodoDto>
            {
                new TodoDto { Id = Guid.NewGuid(), Title = "Test Todo 1" }
            },
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 1
        };

        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetAll(query, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        var returned = Assert.IsType<PagedResult<TodoDto>>(result.Value);
        Assert.Single(returned.Items);
    }
}
