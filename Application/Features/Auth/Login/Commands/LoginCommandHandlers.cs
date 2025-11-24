using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Auth.Login.Commands
{
    public class LoginCommandHandlers : IRequestHandler<LoginCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _tokenService;

        public LoginCommandHandlers(IUnitOfWork unitOfWork, IJwtTokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (!user.IsEmailVerified)
            {
                return "Email is not Verified";
            }

            if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return _tokenService.GenerateUserToken(user);
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}
