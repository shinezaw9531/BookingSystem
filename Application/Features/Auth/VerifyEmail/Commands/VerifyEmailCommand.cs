using MediatR;

namespace Application.Features.Auth.VerifyEmail.Commands
{
    public record VerifyEmailCommand : IRequest<bool>
    {
        public string Token { get; init; }
    }
}
