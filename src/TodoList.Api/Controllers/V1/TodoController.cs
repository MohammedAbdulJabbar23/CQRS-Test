
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Common.Models;
using TodoList.Application.Features.Auth.Command.Login;
using TodoList.Application.Features.Auth.Command.Register;
using TodoList.Application.Features.Todos.Commands.CreateTodo;
using TodoList.Application.Features.Todos.Commands.DeleteTodo;
using TodoList.Application.Features.Todos.Commands.UpdateTodo;
using TodoList.Application.Features.Todos.Queries.GetTodos;

namespace TodoList.Api.COntrollers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly IMediator _mediator;
    public TodoController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TodoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetTodosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteTodoCommand
        {
            Id = id
        });
        return NoContent();
    }
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CompleteTodo(Guid id)
    {
        await _mediator.Send(new CompletedTodoCommand { Id = id });
        return NoContent();
    }
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] UpdateTodoCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }
}