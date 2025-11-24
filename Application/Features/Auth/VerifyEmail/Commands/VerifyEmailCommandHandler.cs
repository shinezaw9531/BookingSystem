using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using MediatR;
using System.Security.Claims;

namespace Application.Features.Auth.VerifyEmail.Commands
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _tokenService;

        public VerifyEmailCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal principal = _tokenService.GetPrincipalFromToken(request.Token, isVerification: true);
            if (principal == null)
            {
                return false;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return false;
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (user.IsEmailVerified)
            {
                return true;
            }

            user.IsEmailVerified = true;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
