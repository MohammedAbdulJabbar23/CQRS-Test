
using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoList.Application.Features.Auth.Command.Login;
using TodoList.Domain.Entities;
using TodoList.Application.Common.Exceptions;

namespace TodoList.Application.Features.Auth.Command.Register;


public class RegisterCommand : IRequest<LoginResponse>
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, LoginResponse>
{
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterCommandHandler(IMediator mediator, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _mediator = mediator;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<LoginResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            throw new ValidationException("Email already exists.");
        }

        var newUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = true,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ValidationException($"Failed to create account: {errors}");
        }

        var role = string.IsNullOrEmpty(request.Role) ? "User" : request.Role;

        if (!await _roleManager.RoleExistsAsync(role))
        {
            throw new ValidationException($"Role '{role}' does not exist.");
        }
        await _userManager.AddToRoleAsync(newUser, role);
        return await _mediator.Send(new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        });
    }
}
