using FluentValidation;

namespace TodoList.Application.Features.Todos.Commands.CreateTodo;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Incorrect priority value");

        // RuleFor(x => x.DueDate)
        //     .GreaterThan(DateTime.UtcNow)
        //     .When(x => x.DueDate.HasValue)
        //     .WithMessage("Due date must be in the future");
    }
}