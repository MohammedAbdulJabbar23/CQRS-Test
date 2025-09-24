
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Features.Auth.Command.Login;
using TodoList.Application.Features.Auth.Command.Register;
using TodoList.Application.Features.Todos.Commands.CreateTodo;

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
}