using Domain.Entity;
using System.Security.Claims;

namespace Application.Interfaces.External
{
    public interface IJwtTokenService
    {
        string GenerateUserToken(User user);
        string GenerateVerificationToken(User user);
        ClaimsPrincipal GetPrincipalFromToken(string token, bool isVerification);
    }

    public interface IExternalEmailService
    {
        Task SendVerifyEmailAsync(string recipientEmail, string verificationLink);
    }

    public interface IScheduledBookingService
    {
        Task ProcessWaitlistAfterCancellation(int classScheduleId);

        Task FinalizeClassAndRefundWaitlist(int classScheduleId);
    }

    //public interface IExternalEmailService
    //{
    //    Task<bool> SendVerifyEmailAsync(string email, string verificationLink);
    //}

    public interface IPaymentService
    {
        bool AddPaymentCard(string userId, string cardNumber);
        bool PaymentCharge(string userId, decimal amount);
    }
}
