using Application.Interfaces.External;

namespace Infrastructure.Services
{
    public class MockPaymentService : IPaymentService
    {
        public bool AddPaymentCard(string userId, string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber)) return false;
            return true;
        }

        public bool PaymentCharge(string userId, decimal amount)
        {
            if (amount <= 0) return false;
            return true;
        }
    }
}
