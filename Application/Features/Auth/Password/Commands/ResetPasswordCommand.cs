using MediatR;

namespace Application.Features.Auth.Password.Commands
{
    public class ResetPasswordCommand : IRequest<Unit>
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
