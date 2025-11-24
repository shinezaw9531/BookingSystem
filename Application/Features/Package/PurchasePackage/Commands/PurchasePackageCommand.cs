using MediatR;

namespace Application.Features.Package.PurchasePackage.Commands
{
    public record PurchasePackageCommand(int UserId, int PackageId, decimal Amount, string CardNumber) : IRequest<string>;
}
