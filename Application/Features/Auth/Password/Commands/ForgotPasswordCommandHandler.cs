using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Auth.Password.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ForgotPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email.ToLower());

            if (user == null)
            {
                return "Password reset link successfully sent (if the user exists).";
            }

            string resetToken = Guid.NewGuid().ToString("N");
            DateTime expiryTime = DateTime.UtcNow.AddHours(1);

            user.ResetToken = resetToken;
            user.ResetTokenExpiry = expiryTime;

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return resetToken;
        }
    }
}
