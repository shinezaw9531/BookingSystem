using MediatR;

namespace Application.Features.Auth.Login.Commands
{
    public record LoginCommand : IRequest<string>
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
