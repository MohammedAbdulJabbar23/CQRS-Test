using MediatR;
using TodoList.Application.Common.Exceptions;
using TodoList.Application.Common.Interfaces;

namespace TodoList.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _tokenService.RefreshTokenAsync(request.RefreshToken);
        
        if (result == null)
            throw new ValidationException("Invalid refresh token");

        return new RefreshTokenResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            AccessTokenExpiry = result.AccessTokenExpiry
        };
    }
}