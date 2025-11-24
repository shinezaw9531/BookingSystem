using Application.DTOs;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Settings;
using Domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Auth.Register.Command
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _tokenService;
        private readonly IExternalEmailService _emailService;
        private readonly ProjectSettings _settings;
        private readonly IConfiguration _configuration;

        public RegisterCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService tokenService, IExternalEmailService emailService, IOptions<ProjectSettings> settings, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _emailService = emailService;
            _settings = settings.Value;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.Users.GetByEmailAsync(request.Email) != null)
            {
                return new AuthResultDto { Success = false, Message = "User with this email already exists." };
            }

            using (var hmac = new HMACSHA512())
            {
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordSalt = hmac.Key,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                    IsEmailVerified = false
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();
                //string url = _settings.BaseURL;
                string verificationToken = _tokenService.GenerateVerificationToken(user);
                string verificationLink = $"{_configuration["Project:BaseURL"]}/api/auth/verify-email?token={verificationToken}";
                await _emailService.SendVerifyEmailAsync(user.Email, verificationLink);

                return new AuthResultDto
                {
                    Success = true,
                    Message = "Registration successful. Please check your email to verify your account.",
                    Token = verificationToken,
                };
            }
        }
    }
}
