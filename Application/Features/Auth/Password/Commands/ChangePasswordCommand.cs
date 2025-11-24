using MediatR;

namespace Application.Features.Auth.Password.Commands
{
    public class ChangePasswordCommand : IRequest<Unit>
    {
        public int UserId { get; set; }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
