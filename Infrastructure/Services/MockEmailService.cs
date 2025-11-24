using Application.Interfaces.External;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class MockEmailService : IExternalEmailService
    {
        private readonly ILogger<MockEmailService> _logger;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendVerifyEmailAsync(string recipientEmail, string verificationLink)
        {
            _logger.LogInformation($"[MOCK EMAIL SERVICE] Sending verification link to {recipientEmail}: {verificationLink}");
            return Task.CompletedTask;
        }
    }
}
