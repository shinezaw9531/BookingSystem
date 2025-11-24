using Application.Interfaces.Repositories;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Auth.Password.Commands
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Token);

            if (user == null)
            {
                throw new ApplicationException("Invalid or expired reset token.");
            }

            if (user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                user.ResetToken = null;
                user.ResetTokenExpiry = null;
                _userRepository.Update(user);
                await _unitOfWork.CompleteAsync(cancellationToken);

                throw new ApplicationException("Reset token has expired. Please request a new password reset.");
            }

            CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Unit.Value;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
