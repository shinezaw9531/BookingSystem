using MediatR;

namespace Application.Features.Auth.Password.Commands
{
    public class ForgotPasswordCommand : IRequest<string>
    {
        public string Email { get; set; }
    }
}
