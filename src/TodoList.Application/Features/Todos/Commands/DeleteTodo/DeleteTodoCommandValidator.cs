using FluentValidation;

namespace TodoList.Application.Features.Todos.Commands.DeleteTodo;

public class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Todo ID is required");
    }
}