using FluentValidation;
using TodoList.Domain.Enums;

namespace TodoList.Application.Features.Todos.Commands.UpdateTodo;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Todo ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("incorrect priority value");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("not corrent status value");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDate.HasValue && x.Status != TodoStatus.Completed)
            .WithMessage("Due date must be in the future for non-completed todos");
    }
}