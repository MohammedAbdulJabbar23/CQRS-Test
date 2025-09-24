

using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoList.Application.Common.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Application.Common.Exceptions;

namespace TodoList.Application.Features.Auth.Command.Login;


public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public List<String> Roles { get; set; } = new();
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new ValidationException("incoreect email or password");
        }
        if (!user.IsActive)
        {
            throw new ValidationException("Invalid email or password");
        }
        var res = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false); ;
        if (!res.Succeeded)
        {

            throw new ValidationException("Invalid email or password");
        }
        var tokens = await _tokenService.GenerateTokensAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return new LoginResponse
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiry = tokens.AccessTokenExpiry,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };

    }
}