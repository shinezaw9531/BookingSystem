using Application.DTOs;
using MediatR;

namespace Application.Features.Auth.Register.Command
{
    public record RegisterCommand : IRequest<AuthResultDto>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
