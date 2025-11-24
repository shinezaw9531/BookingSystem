using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Domain;
using Domain.Entity;
using MediatR;

namespace Application.Features.Package.PurchasePackage.Commands
{
    public class PurchasePackageCommandHandler : IRequestHandler<PurchasePackageCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public PurchasePackageCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task<string> Handle(PurchasePackageCommand request, CancellationToken cancellationToken)
        {
            var package = await _unitOfWork.Packages.GetByIdAsync(request.PackageId)
                ?? throw new NotFoundException(nameof(Package), request.PackageId);

            if (package.Price != request.Amount)
                throw new BusinessRuleException("Price mismatch. Cannot proceed with payment.");

            bool paymentSuccess = _paymentService.PaymentCharge(request.UserId.ToString(), request.Amount);

            if (!paymentSuccess)
                return "Payment failed. Please check your card details or try again.";

            var purchaseDate = DateTime.UtcNow;
            //var expirationDate = purchaseDate.AddDays(package.DurationDays);
            var expirationDate = purchaseDate.AddDays(1);

            var userPackage = new UserPackage
            {
                UserId = request.UserId,
                PackageId = request.PackageId,
                RemainingCredits = package.InitialCredits,
                PurchaseDate = purchaseDate,
                ExpiryDate = expirationDate
            };

            await _unitOfWork.UserPackages.AddAsync(userPackage);
            await _unitOfWork.CompleteAsync();

            return "Package purchased successfully.";
        }
    }
}
